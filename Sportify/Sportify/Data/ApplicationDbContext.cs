using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Sportify.Models;

namespace Sportify.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Gym> Salonlar { get; set; }
        public DbSet<Service> Servisler { get; set; }
        public DbSet<ApplicationUser> Kullanıcılar { get; set; }
    }
}
