# BigBrainID
Big Brain Authorization Service - oAuth 2.0

![build](https://img.shields.io/github/workflow/status/BigBrain-Solutions/BigBrainID/.NET/main)
![issues](https://img.shields.io/github/issues-raw/BigBrain-Solutions/BigBrainID)
![tests](https://github.com/BigBrain-Solutions/BigBrainID/actions/workflows/dotnet-tests.yml/badge.svg)

## About

- Identity Service for <a href="https://github.com/BigBrain-Solutions/bbs_website"> BigBrain Solution website </a> made in ASP.NET CORE

## Architecture

<p> Work in progress... </p>

<img src="https://raw.githubusercontent.com/BigBrain-Solutions/graphs/main/bigbrain_solutions_web.png" width="70%" />

## Run

#### Run by docker compose:
```
/BigBrainID> docker compose-up
```

#### On Azure

``
/BigBrainID/Application/Helpers
``

In ``CassandraConnectionHelper.cs``

Should look like this: 

```csharp
public static Cluster Connect(CassandraSettings cassandraSettings)
{
    var options = new SSLOptions(SslProtocols.Tls12, true, (sender, certificate, chain, errors) => true);
    options.SetHostNameResolver((ipAddress) => cassandraSettings.IpAddress);
    var cluster = Cluster.Builder()
        .WithCredentials(cassandraSettings.Username, cassandraSettings.Password)
        .WithPort(cassandraSettings.Port)
        .AddContactPoint(cassandraSettings.IpAddress)
        .WithSSL(options)
        .Build();
    
    return cluster;
}
```

Open ``appsettings.json``

Edit
```json
"CassandraSettings": {
"IpAddress": "<db>.cassandra.cosmos.azure.com",
"Username": "<username>",
"Password": "<your_password>",
"Port": 10350
}
```

## Tools

- ASP.NET CORE
- Cassandra
