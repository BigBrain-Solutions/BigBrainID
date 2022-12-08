namespace Domain.Models;

public class CassandraSettings
{
    public bool OnAzure { get; set; }
    public string IpAddress { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public int Port { get; set; }
}