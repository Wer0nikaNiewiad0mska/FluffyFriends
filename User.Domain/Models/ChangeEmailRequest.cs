namespace User.Domain.Models;

public class ChangeEmailRequest
{
    public string NewEmail { get; set; } = default!;
    public string Password { get; set; } = default!;
}
