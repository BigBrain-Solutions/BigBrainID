using Application.Helpers;
using Cassandra;
using Cassandra.Mapping;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

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
    
    [HttpPatch]
    public async Task<IActionResult> Update(User user)
    {
        await _session.ExecuteAsync(new SimpleStatement($"UPDATE BBS_ID.users SET access_token = '{user.AccessToken}' WHERE id = {user.Id}"));
        return Ok();
    }
}