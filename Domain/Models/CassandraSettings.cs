namespace Domain.Models;

public class CassandraSettings
{
    public string IpAddress { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public int Port { get; set; }
}