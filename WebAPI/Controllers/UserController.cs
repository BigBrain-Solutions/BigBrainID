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

    public UserController()
    {
        _cluster = CassandraConnectionHelper.Connect();

        _session = _cluster.Connect();

        _mapper = new Mapper(_session);
    }

    [HttpGet]
    public IActionResult Me([FromHeader] string Authorization)
    {
        var bearer = new Regex("Bearer ");

        var accessToken = bearer.Split(Authorization);

        var user = _mapper.First<User>($"SELECT * FROM BBS_ID.users WHERE access_token = '{accessToken[1]}' ALLOW FILTERING");
        
        return Ok(user);
    }
}