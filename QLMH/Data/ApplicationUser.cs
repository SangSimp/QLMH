using Microsoft.AspNetCore.Identity;

namespace QLMH.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string? Address { get; set; } 
    }
}
