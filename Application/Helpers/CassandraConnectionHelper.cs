using System.Security.Authentication;
using Cassandra;
using Domain.Models;
using Newtonsoft.Json;

namespace Application.Helpers;

public static class CassandraConnectionHelper
{
    public static Cluster Connect()
    {
        var directory = Environment.CurrentDirectory;

        var settings = JsonConvert.DeserializeObject<CassandraSettings>(File.ReadAllText(directory + "/cassandrasettings.json"));
        
        var options = new SSLOptions(SslProtocols.Tls12, true, (sender, certificate, chain, errors) => true);
        options.SetHostNameResolver((ipAddress) => settings.IpAddress);
        var cluster = Cluster.Builder()
            .WithCredentials(settings.Username, settings.Password)
            .WithPort(settings.Port)
            .AddContactPoint(settings.IpAddress)
            .WithSSL(options)
            .Build();

        return cluster;
    }
}