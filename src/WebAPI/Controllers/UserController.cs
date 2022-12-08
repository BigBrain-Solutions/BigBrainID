using System.IdentityModel.Tokens.Jwt;
using System.Text.RegularExpressions;
using Application.Helpers;
using Cassandra;
using Cassandra.Mapping;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly ICluster _cluster;
    private readonly Cassandra.ISession _session;
    private readonly IMapper _mapper;

    public UserController(CassandraSettings cassandraSettings)
    {
        _cluster = CassandraConnectionHelper.Connect(cassandraSettings);

        _session = _cluster.Connect();

        _mapper = new Mapper(_session);
    }

    [HttpGet]
    public IActionResult Me([FromHeader] string Authorization)
    {
        var bearer = new Regex("Bearer ");

        var accessToken = bearer.Split(Authorization)[1];

        var user = _mapper.First<User>($"SELECT * FROM BBS_ID.users WHERE access_token = '{accessToken}' ALLOW FILTERING");
        
        var handler = new JwtSecurityTokenHandler();
        var accessTokenDecoded = handler.ReadJwtToken(accessToken);

        if (accessTokenDecoded.ValidTo.ToUniversalTime() < DateTime.Now.ToUniversalTime())
        {
            return Unauthorized();
        }
        
        var claims = accessTokenDecoded.Claims;
        var scopes = claims.ToList()[1].Value;

        if (!scopes.Contains("read:user-profile"))
        {
            return BadRequest(new {Error = "Wrong scopes"});
        }
        
        return Ok(new
        {
            user.Username,
            user.Email,
            user.Image
        });
    }
}