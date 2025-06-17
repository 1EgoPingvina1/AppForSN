using DTC.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DTC.Infrastructure.Data
{
    public class ApplicationDataBaseContext : IdentityDbContext<User, Role, int>
    {
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public ApplicationDataBaseContext(DbContextOptions<ApplicationDataBaseContext> options)
            : base(options) { }

        public ApplicationDataBaseContext() { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(rt => rt.Id);
                entity.HasOne(rt => rt.User)
                    .WithMany(u => u.RefreshTokens)
                    .HasForeignKey(rt => rt.UserId);
            });
        }
    }
}
