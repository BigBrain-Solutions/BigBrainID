using Application.Helpers;
using Cassandra;
using Cassandra.Mapping;
using Domain.Dtos;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly ICluster _cluster;
    private readonly Cassandra.ISession _session;
    private readonly IMapper _mapper;

    public AuthenticationController()
    {
        _cluster = CassandraConnectionHelper.Connect();

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

        var sql = $@"INSERT INTO BBS_ID.users (id, password_hash, salt, email, access_token, refresh_token) values ({Guid.NewGuid()}, '{user.PasswordHash}', '{user.Salt}', '{request.Email}', '', '')";

        await _session.ExecuteAsync(new SimpleStatement(sql));

        return Ok(request);
    }
}