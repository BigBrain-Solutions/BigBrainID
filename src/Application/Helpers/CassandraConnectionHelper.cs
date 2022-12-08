using System.Security.Authentication;
using Cassandra;
using Domain.Models;

namespace Application.Helpers;

public static class CassandraConnectionHelper
{
    public static Cluster Connect(CassandraSettings cassandraSettings)
    {
        Cluster cluster;
        
        // Azure
        if (cassandraSettings.OnAzure)
        {
            var options = new SSLOptions(SslProtocols.Tls12, true, (sender, certificate, chain, errors) => true);
            options.SetHostNameResolver((ipAddress) => cassandraSettings.IpAddress);
            cluster = Cluster.Builder()
                .WithCredentials(cassandraSettings.Username, cassandraSettings.Password)
                .WithPort(cassandraSettings.Port)
                .AddContactPoint(cassandraSettings.IpAddress)
                .WithSSL(options)
                .Build();

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Connecting to Cassandra on Azure");
            Console.ResetColor();
            
            return cluster;
        }
        
        // Docker
        cluster = Cluster.Builder().AddContactPoint("127.0.0.1").Build();
        
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Connecting to Cassandra on Docker");
        Console.ResetColor();
        
        return cluster;
    }
}