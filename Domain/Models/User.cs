namespace Domain.Models;

public class User
{
    public Guid Id { get; set; }
    public string PasswordHash { get; set; } = default!;
    public string Salt { get; set; } = default!;
    
    public string? AccessToken { get; set; } = default!;
    public string? RefreshToken { get; set; } = default!;
    
    public IEnumerable<Guid> ApplicationIds { get; set; } = default!;
}