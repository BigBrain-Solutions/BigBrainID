using System.Security.Authentication;
using Cassandra;
using Domain.Models;

namespace Application.Helpers;

public static class CassandraConnectionHelper
{
    public static Cluster Connect(CassandraSettings cassandraSettings)
    {
        var options = new SSLOptions(SslProtocols.Tls12, true, (sender, certificate, chain, errors) => true);
        /*
        options.SetHostNameResolver((ipAddress) => cassandraSettings.IpAddress);
        var cluster = Cluster.Builder()
            .WithCredentials(cassandraSettings.Username, cassandraSettings.Password)
            .WithPort(cassandraSettings.Port)
            .AddContactPoint(cassandraSettings.IpAddress)
            .WithSSL(options)
            .Build();
            */

        var cluster = Cluster.Builder().AddContactPoint("127.0.0.1");

        return cluster.Build();
    }
}