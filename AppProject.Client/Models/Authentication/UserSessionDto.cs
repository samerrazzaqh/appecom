namespace AppProject.Client.Models.Authentication;

public class UserSessionDto
{
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public bool IsAuthenticated { get; set; }
    public string UserId { get; set; }
    public string Email { get; set; }
    public List<string> Roles { get; set; }
    public Dictionary<string, string> Claims { get; set; } = new();
    public long? TokenExpired { get; set; }
}