using Application.Helpers;
using Cassandra;
using Cassandra.Mapping;
using Domain.Dtos;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApplicationController : ControllerBase
{
    private readonly ICluster _cluster;
    private readonly Cassandra.ISession _session;
    private readonly IMapper _mapper;

    public ApplicationController()
    {
        _cluster = CassandraConnectionHelper.Connect();
        
        _session = _cluster.Connect();

        _mapper = new Mapper(_session);
    }
    
    [HttpPut]
    public async Task<IActionResult> Create([FromBody] AppCreateRequest request)
    {
        var appId = Guid.NewGuid();
        var app = $"INSERT INTO BBS_ID.applications (id, title, client_id, client_secret, owner_id) VALUES ({appId}, '{request.Title}', '{CredentialsHelper.GenerateClientId()}', '{CredentialsHelper.GenerateClientSecret()}', {request.OwnerId})";

        var userSql =
            $"UPDATE BBS_ID.users SET application_ids = application_ids + {{'{appId.ToString()}'}} WHERE id = {request.OwnerId}";
        
        await _session.ExecuteAsync(new SimpleStatement(app));
        await _session.ExecuteAsync(new SimpleStatement(userSql));

        return Created("",new{ Message = "Application was created"});
    }
}