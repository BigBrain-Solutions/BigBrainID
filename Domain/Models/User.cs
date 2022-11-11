using Cassandra.Mapping.Attributes;

namespace Domain.Models;

public class User
{
    public Guid Id { get; set; }
    
    [Column("password_hash")]
    public string PasswordHash { get; set; } = default!;
    public string Salt { get; set; } = default!;
    
    [Column("access_token")]
    public string? AccessToken { get; set; } = default!;
    [Column("refresh_token")]
    public string? RefreshToken { get; set; } = default!;
    public string Email { get; set; } = default!;
}