using Application.Helpers;
using Cassandra;
using Cassandra.Mapping;
using Domain.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorizeController : ControllerBase
{
    private readonly ICluster _cluster;
    private readonly Cassandra.ISession _session;
    private readonly IMapper _mapper;

    public AuthorizeController()
    {
        _cluster = CassandraConnectionHelper.Connect();

        _session = _cluster.Connect();

        _mapper = new Mapper(_session);
    }
    
    [HttpGet]
    public async Task<IActionResult> Callback(string email, string clientId, string redirectUri, string scope)
    {
        // Check if clientId is valid
        var cId = await _mapper.FirstOrDefaultAsync<string>($"SELECT client_id FROM BBS_ID.applications WHERE client_id = '{clientId}' ALLOW FILTERING");
        if (string.IsNullOrEmpty(cId))
        {
            return BadRequest(new{ Error = "client_id was not found."});
        }
        
        // TODO: check if redirectUri is on the list

        var code = CredentialsHelper.GenerateCode().ToString();

        var userId = _mapper.First<Guid>($"SELECT id FROM BBS_ID.users WHERE email = '{email}' ALLOW FILTERING");
        await _session.ExecuteAsync(new SimpleStatement($"INSERT INTO BBS_ID.codes (id, code, email, scopes) VALUES ({userId},'{code}', '{email}', '{scope}')"));
        
        redirectUri += $"?code={code}";
        
        return Redirect(redirectUri);
    }

    [HttpPost("token")]
    public IActionResult GrantToken([FromBody] GrantTokenRequest request, [FromHeader] string clientId, [FromHeader] string clientSecret)
    {
        // TODO: Check if Code is valid and get the scopes
        // TODO: Check Grant Type
        
        // Generate Access Token and save it to database
        var accessToken = AuthenticationHelper.GenerateAccessToken(AuthenticationHelper.AssembleClaimsIdentity(request.Code));
        
        var userId = _mapper.First<Guid>($"SELECT id FROM BBS_ID.codes WHERE code = '{request.Code}' ALLOW FILTERING");

        _session.ExecuteAsync(new SimpleStatement($"UPDATE BBS_ID.users SET access_token='{accessToken}' WHERE id = {userId}"));
        
        // Generate Refresh Token
        var refreshToken = AuthenticationHelper.GenerateAccessToken(AuthenticationHelper.AssembleClaimsIdentity(Random.Shared.Next().ToString()));

        _session.ExecuteAsync(new SimpleStatement($"UPDATE BBS_ID.users SET refresh_token='{refreshToken}' WHERE id = {userId}"));
        
        // TODO: Check for client id or client secret

        return Ok(new
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        });
    }
}