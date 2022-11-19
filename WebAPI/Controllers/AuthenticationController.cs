using Application.Helpers;
using Cassandra;
using Cassandra.Mapping;
using Domain.Dtos;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthenticationController : ControllerBase
{
    private readonly Settings _settings;
    private readonly ICluster _cluster;
    private readonly Cassandra.ISession _session;
    private readonly IMapper _mapper;

    public AuthenticationController(CassandraSettings cassandraSettings, Settings settings)
    {
        _settings = settings;
        _cluster = CassandraConnectionHelper.Connect(cassandraSettings);

        _session = _cluster.Connect();

        _mapper = new Mapper(_session);
    }
    
    [HttpPut]
    public async Task<IActionResult> Put([FromBody] SignUpRequest request)
    {
        if (request.Email is null || request.Password is null)
        {
            return BadRequest();
        }

        if ((await _mapper.FetchAsync<string>("SELECT email FROM BBS_ID.users")).Any(email => email == request.Email))
        {
            return BadRequest(new { Error = "Email exists." });
        }
        
        var user = new User
        {
            PasswordHash = request.Password
        };
        
        user.ProvideSaltAndHash();

        var sql = $@"INSERT INTO BBS_ID.users (id, password_hash, salt, email, access_token, refresh_token, username) values ({Guid.NewGuid()}, '{user.PasswordHash}', '{user.Salt}', '{request.Email}', '', '', '{request.Username}')";

        await _session.ExecuteAsync(new SimpleStatement(sql));

        return Created(string.Empty, new {Message = "Account was created"});
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, [FromHeader] string cId)
    {
        var user = _mapper.First<User>($"SELECT * FROM BBS_ID.users WHERE email = '{request.Email}' ALLOW FILTERING");

        const string scope = "read:user-profile write:user-profile";
        
        // Generate Access Token and save it to database
        var accessToken = AuthenticationHelper.GenerateAccessToken(_settings, AuthenticationHelper.AssembleClaimsIdentity(user.Id, scope, cId), 99350);
        await _session.ExecuteAsync(new SimpleStatement($"UPDATE BBS_ID.users SET access_token='{accessToken}' WHERE id = {user.Id}")).ConfigureAwait(false);

        if (user is null)
            return BadRequest(new {Error = $"Email: '{request.Email}' does not exists in our database"});


        if (user.PasswordHash != AuthenticationHelper.GenerateHash(request.Password, user.Salt))
            return BadRequest(new {Error = "Invalid password"});
        
        return Ok(new
        {
            AccessToken = accessToken
        });
    }
}