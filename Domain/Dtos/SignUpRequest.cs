namespace Domain.Dtos;

public class SignUpRequest
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
}