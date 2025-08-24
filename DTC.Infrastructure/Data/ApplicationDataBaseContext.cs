using DTC.Domain.Entities.Identity;
using DTC.Domain.Entities.Main;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DTC.Infrastructure.Data
{
    public class ApplicationDataBaseContext : IdentityDbContext<User, Role, int>
    {
        //Authorization domain
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        /// <summary>
        /// Project / Authors Domain (main)
        /// </summary>
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectType> ProjectTypes { get; set; }
        public DbSet<ProjectFile> ProjectFiles { get; set; }
        public DbSet<ProjectStatus> Statuses { get; set; }

        public DbSet<Author> Authors { get; set; }
        public DbSet<AuthorGroup> AuthorGroups { get; set; }
        public DbSet<AuthorGroupMember> AuthorGroupsMember { get; set; }

        public ApplicationDataBaseContext(DbContextOptions<ApplicationDataBaseContext> options)
            : base(options) { }
        
        public ApplicationDataBaseContext() { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ProjectStatus>().HasData(
            new ProjectStatus { Id = 1, Name = "Registered" },
            new ProjectStatus { Id = 2, Name = "InReview" },
            new ProjectStatus { Id = 3, Name = "Approved" },
            new ProjectStatus { Id = 4, Name = "Rejected" });

            builder.Entity<UserRoles>(entity =>
            {
                entity.HasOne(ur => ur.User)
                    .WithMany(u => u.Roles)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();

                entity.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();
            });
            builder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(rt => rt.Id);
                entity.HasOne(rt => rt.User)
                    .WithMany(u => u.RefreshTokens)
                    .HasForeignKey(rt => rt.UserId);
            });

            builder.Entity<AuthorGroupMember>(entity =>
            {
                entity.HasKey(agm => new { agm.Author_ID, agm.AuthorGroup_ID });

                entity.HasOne(agm => agm.Author)
                    .WithMany(a => a.GroupMemberships)
                    .HasForeignKey(agm => agm.Author_ID);

                entity.HasOne(agm => agm.AuthorGroup)
                    .WithMany(g => g.Members)
                    .HasForeignKey(agm => agm.AuthorGroup_ID);
            });
        }
    }
}