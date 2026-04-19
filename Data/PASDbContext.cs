using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PUSL2020_Coursework.Models;

namespace PUSL2020_Coursework.Data
{
    public class PASDbContext : IdentityDbContext<ApplicationUser>
    {
        public PASDbContext(DbContextOptions<PASDbContext> options)
            : base(options)
        {
        }

        public DbSet<StudentProfile> StudentProfiles { get; set; } = null!;
        public DbSet<SupervisorProfile> SupervisorProfiles { get; set; } = null!;
        public DbSet<ResearchArea> ResearchAreas { get; set; } = null!;
        public DbSet<SupervisorExpertise> SupervisorExpertises { get; set; } = null!;
        public DbSet<Project> Projects { get; set; } = null!;
        public DbSet<Match> Matches { get; set; } = null!;
        public DbSet<GroupMember> GroupMembers { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Rename Identity tables to have cleaner names
            modelBuilder.Entity<ApplicationUser>().ToTable("Users");
            modelBuilder.Entity<IdentityRole>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");

            // Configure StudentProfile
            modelBuilder.Entity<StudentProfile>()
                .HasOne(s => s.User)
                .WithMany(u => u.StudentProfiles)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StudentProfile>()
                .HasMany(s => s.Projects)
                .WithOne(p => p.StudentProfile)
                .HasForeignKey(p => p.StudentProfileId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure SupervisorProfile
            modelBuilder.Entity<SupervisorProfile>()
                .HasOne(s => s.User)
                .WithMany(u => u.SupervisorProfiles)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure SupervisorExpertise
            modelBuilder.Entity<SupervisorExpertise>()
                .HasOne(se => se.SupervisorProfile)
                .WithMany(s => s.ExpertiseAreas)
                .HasForeignKey(se => se.SupervisorProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SupervisorExpertise>()
                .HasOne(se => se.ResearchArea)
                .WithMany(r => r.SupervisorExpertises)
                .HasForeignKey(se => se.ResearchAreaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Project
            modelBuilder.Entity<Project>()
                .HasOne(p => p.ResearchArea)
                .WithMany(r => r.Projects)
                .HasForeignKey(p => p.ResearchAreaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Project>()
                .HasOne(p => p.SubmittedByUser)
                .WithMany(u => u.Projects)
                .HasForeignKey(p => p.SubmittedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Match
            modelBuilder.Entity<Match>()
                .HasOne(m => m.Project)
                .WithMany(p => p.Matches)
                .HasForeignKey(m => m.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.SupervisorProfile)
                .WithMany(s => s.Matches)
                .HasForeignKey(m => m.SupervisorProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure GroupMember
            modelBuilder.Entity<GroupMember>()
                .HasOne(gm => gm.StudentProfile)
                .WithMany(s => s.GroupMembers)
                .HasForeignKey(gm => gm.StudentProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure indices
            modelBuilder.Entity<Project>()
                .HasIndex(p => p.Status);

            modelBuilder.Entity<Match>()
                .HasIndex(m => m.Status);

            modelBuilder.Entity<ResearchArea>()
                .HasIndex(r => r.IsActive);

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Research Areas
            var researchAreas = new[]
            {
                new ResearchArea { Id = 1, Name = "Artificial Intelligence", Description = "Machine Learning, Deep Learning, NLP", IsActive = true },
                new ResearchArea { Id = 2, Name = "Cybersecurity", Description = "Network Security, Cryptography, Penetration Testing", IsActive = true },
                new ResearchArea { Id = 3, Name = "Web Development", Description = "Frontend, Backend, Full-stack Development", IsActive = true },
                new ResearchArea { Id = 4, Name = "Cloud Computing", Description = "AWS, Azure, Google Cloud, Distributed Systems", IsActive = true },
                new ResearchArea { Id = 5, Name = "Data Science", Description = "Big Data, Data Analytics, Visualization", IsActive = true },
                new ResearchArea { Id = 6, Name = "IoT & Embedded Systems", Description = "Arduino, Raspberry Pi, Embedded Linux", IsActive = true },
                new ResearchArea { Id = 7, Name = "Blockchain", Description = "Cryptocurrency, Smart Contracts, DeFi", IsActive = true },
                new ResearchArea { Id = 8, Name = "Game Development", Description = "Unity, Unreal Engine, Game Design", IsActive = true }
            };

            modelBuilder.Entity<ResearchArea>().HasData(researchAreas);

            // Seed default roles
            var roles = new[]
            {
                new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = "2", Name = "ModuleLeader", NormalizedName = "MODULELEADER" },
                new IdentityRole { Id = "3", Name = "Supervisor", NormalizedName = "SUPERVISOR" },
                new IdentityRole { Id = "4", Name = "Student", NormalizedName = "STUDENT" }
            };

            modelBuilder.Entity<IdentityRole>().HasData(roles);
        }
    }
}
