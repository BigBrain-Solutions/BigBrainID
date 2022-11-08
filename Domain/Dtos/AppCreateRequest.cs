namespace Domain.Dtos;

public class AppCreateRequest
{
    public Guid OwnerId { get; set; }
    public string Title { get; set; } = default!;
}