using System.Text.RegularExpressions;
using Application.Helpers;
using Cassandra;
using Cassandra.Mapping;
using Domain.Dtos;
using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/oauth2")]
public class AuthorizeController : ControllerBase
{
    private readonly Settings _settings;
    private readonly ICluster _cluster;
    private readonly Cassandra.ISession _session;
    private readonly IMapper _mapper;

    public AuthorizeController(Settings settings, CassandraSettings cassandraSettings)
    {
        _settings = settings;
        _cluster = CassandraConnectionHelper.Connect(cassandraSettings);

        _session = _cluster.Connect();

        _mapper = new Mapper(_session);
    }
    
    [HttpGet]
    public async Task<IActionResult> Callback(string email, string clientId, string redirectUri, string scope, string responseType)
    {
        // Check if clientId is valid
        var cId = await _mapper.FirstOrDefaultAsync<string>($"SELECT client_id FROM BBS_ID.applications WHERE client_id = '{clientId}' ALLOW FILTERING");
        if (string.IsNullOrEmpty(cId))
        {
            return BadRequest(new{ Error = "client_id was not found."});
        }
        
        var userId = _mapper.First<Guid>($"SELECT id FROM BBS_ID.users WHERE email = '{email}' ALLOW FILTERING");
        
        // TODO: check if redirectUri is on the list

        // Generate Access Token and save it to database
        var accessToken = AuthenticationHelper.GenerateAccessToken(_settings, AuthenticationHelper.AssembleClaimsIdentity(userId, scope));
        await _session.ExecuteAsync(new SimpleStatement($"UPDATE BBS_ID.users SET access_token='{accessToken}' WHERE id = {userId}"));
        
        // Generate Refresh Token
        var refreshToken = AuthenticationHelper.GenerateAccessToken(_settings, AuthenticationHelper.AssembleClaimsIdentity(userId, scope));
        await _session.ExecuteAsync(new SimpleStatement($"UPDATE BBS_ID.users SET refresh_token='{refreshToken}' WHERE id = {userId}"));
        
        if (responseType == EResponseType.Token.ToString())
        {
            redirectUri += $"?access_token={accessToken}&refresh_token={refreshToken}";
            
            return Redirect(redirectUri);
        }
        
        var code = CredentialsHelper.GenerateCode().ToString();
        
        await _session.ExecuteAsync(new SimpleStatement($"INSERT INTO BBS_ID.codes (id, code, email, scopes) VALUES ({userId},'{code}', '{email}', '{scope}')"));
        
        redirectUri += $"?code={code}";
        
        return Redirect(redirectUri);
    }

    [HttpPost("token")]
    public async Task<IActionResult> GrantToken([FromBody] GrantTokenRequest request, [FromHeader] string clientId, [FromHeader] string clientSecret)
    {
        var userId = _mapper.First<Guid>($"SELECT id FROM BBS_ID.codes WHERE code = '{request.Code}' ALLOW FILTERING");

        var title = await _mapper.FirstAsync<string>($"SELECT title FROM BBS_ID.applications WHERE client_id = '{clientId}' AND client_secret = '{clientSecret}' ALLOW FILTERING");

        if (string.IsNullOrEmpty(title))
        {
            return BadRequest();
        }
        
        // Check if Code is valid
        if (userId == Guid.Empty)
        {
            return BadRequest(new {Error = "Wrong code"});
        }

        var accessToken = await _mapper.FirstAsync<string>($"SELECT access_token FROM BBS_ID.users WHERE id = {userId}");
        
        // Generate Refresh Token
        var refreshToken = await _mapper.FirstAsync<string>($"SELECT refresh_token FROM BBS_ID.users WHERE id = {userId}");
        
        // TODO: Check for client id or client secret

        return Ok(new
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        });
    }
    
    [HttpPatch("token")]
    public async Task<IActionResult> RefreshToken([FromHeader] string Authorization, [FromQuery] string scope)
    {
        var bearer = new Regex("Bearer ");

        var accessToken = bearer.Split(Authorization)[1];

        var user = _mapper.First<User>($"SELECT * FROM BBS_ID.users WHERE access_token = '{accessToken}' ALLOW FILTERING");
        
        // Generate Access Token and save it to database
        accessToken = AuthenticationHelper.GenerateAccessToken(_settings, AuthenticationHelper.AssembleClaimsIdentity(user.Id, scope));
        await _session.ExecuteAsync(new SimpleStatement($"UPDATE BBS_ID.users SET access_token='{accessToken}' WHERE id = {user.Id}"));
        
        // Generate Refresh Token
        var refreshToken = AuthenticationHelper.GenerateAccessToken(_settings, AuthenticationHelper.AssembleClaimsIdentity(user.Id, scope));
        await _session.ExecuteAsync(new SimpleStatement($"UPDATE BBS_ID.users SET refresh_token='{refreshToken}' WHERE id = {user.Id}"));

        return Ok(new
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        });
    }
}