using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Netdrop.Models
{
    public class NetdropContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<SavedCredentials> SavedCredentials { get; set; }
        public NetdropContext(DbContextOptions<NetdropContext> options) : base (options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);

            builder.Entity<SavedCredentials>()
                .HasOne(p => p.ApplicationUser)
                .WithMany(b => b.SavedCredentials)
                .HasForeignKey(t => t.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApplicationUser>()
                .Navigation(b => b.SavedCredentials)
                .UsePropertyAccessMode(PropertyAccessMode.Property);
        }
    }
}
