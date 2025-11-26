using Microsoft.AspNetCore.Identity;

namespace Sportify.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
    }
}
