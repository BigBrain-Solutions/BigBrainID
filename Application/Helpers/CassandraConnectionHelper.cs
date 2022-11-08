using System.Security.Authentication;
using Cassandra;

namespace Application.Helpers;

public static class CassandraConnectionHelper
{
    public static Cluster Connect(string address, string username, string password, int port)
    {
        var options = new SSLOptions(SslProtocols.Tls12, true, (sender, certificate, chain, errors) => true);
        options.SetHostNameResolver((ipAddress) => address);
        var cluster = Cluster.Builder()
            .WithCredentials(username, password)
            .WithPort(port)
            .AddContactPoint(address)
            .WithSSL(options)
            .Build();

        return cluster;
    }
}