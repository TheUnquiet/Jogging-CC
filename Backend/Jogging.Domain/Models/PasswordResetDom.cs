namespace Jogging.Domain.Models;

public class PasswordResetDom
{
    public string recovery_token { get; set; }
    public string newPassword { get; set; }
    public string email { get; set; }
}