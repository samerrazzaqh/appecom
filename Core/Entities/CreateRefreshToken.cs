using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

public class CreateRefreshToken: BaseEntity<long>
{
    public string RefreshToken { get; set; }
    
    public string UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public virtual AppUser AppUser { get; set; }
}