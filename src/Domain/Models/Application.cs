namespace Domain.Models;

public class Application
{
    public Guid Id { get; set; }

    public string Title { get; set; } = default!;
    
    public string ClientId { get; set; } = default!;
    public string ClientSecret { get; set; } = default!;
    
    public IEnumerable<string>? RedirectUris { get; set; }
    public Guid OwnerId { get; set; } = default!;
}