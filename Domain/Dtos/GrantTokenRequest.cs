namespace Domain.Dtos;

public class GrantTokenRequest
{
    public string Code { get; set; } = default!;
    public string RedirectUri { get; set; } = default!;
    public string GrantType { get; set; } = default!;
}