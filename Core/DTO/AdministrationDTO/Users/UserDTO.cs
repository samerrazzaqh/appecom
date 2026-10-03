namespace Core.DTO.AdministrationDTO.Users;

public class UserDTO
{
    public List<UserResponseDTO>? UserRSDTO { get; set; }
    public string? SearchTerm { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 5;
    public int TotalCount { get; set; }
}


public class UserResponseDTO
{
    public string? UserId { get; set; }
    public string? UserName { get; set; }
}

