using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Sportify.Models;
using Sportify.ViewModels;

namespace Sportify.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Gym> Salonlar { get; set; }
        public DbSet<Trainer> Egitmenler { get; set; }
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Appointment> Randevular { get; set; }
        public DbSet<Service> Servisler { get; set; }

    }
}
