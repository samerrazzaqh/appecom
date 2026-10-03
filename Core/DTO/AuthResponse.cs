namespace Core.DTO;

public class AuthResponse
{
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public long TokenExpired { get; set; }
    public string Message { get; set; }
}