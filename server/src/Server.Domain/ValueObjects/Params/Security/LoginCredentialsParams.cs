namespace Server.Domain.ValueObjects.Params.Security;

public class LoginCredentialsParams
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public string IpAddress { get; set; }
    public string UserAgent { get; set; }
    public bool RememberMe { get; set; } = false;
}
