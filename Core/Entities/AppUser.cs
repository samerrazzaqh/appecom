
using Microsoft.AspNetCore.Identity;

namespace Core.Entities
{
    public class AppUser : IdentityUser
    {
        public string DispalyName { get; set; }
        public Address Address { get; set; }
    }
}
