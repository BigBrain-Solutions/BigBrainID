using System.Security.Authentication;
using Application.Helpers;
using Cassandra;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

const string keyspace = "BBS_ID";

var options = new SSLOptions(SslProtocols.Tls12, true, (sender, certificate, chain, errors) => true);
options.SetHostNameResolver((ipAddress) => "luuqe.cassandra.cosmos.azure.com");
var cluster = CassandraConnectionHelper.Connect();

var session = cluster.Connect();

await session.ExecuteAsync(new SimpleStatement("CREATE KEYSPACE IF NOT EXISTS " + keyspace + " WITH REPLICATION = { 'class' : 'NetworkTopologyStrategy', 'datacenter1' : 1 };"));
await session.ExecuteAsync(new SimpleStatement("CREATE TABLE IF NOT EXISTS BBS_ID.users (id uuid PRIMARY KEY, password_hash text, salt text, access_token text, refresh_token text, email text)"));
await session.ExecuteAsync(new SimpleStatement($"CREATE TABLE IF NOT EXISTS {keyspace}.applications (id uuid PRIMARY KEY, title text, client_id text, client_secret text, redirect_uris set<text>, owner_id uuid), owner_id uuid"));
await session.ExecuteAsync(new SimpleStatement($"CREATE TABLE IF NOT EXISTS {keyspace}.codes (id uuid PRIMARY KEY, code text, email text, scopes text)"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();