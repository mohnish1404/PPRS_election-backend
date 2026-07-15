using Microsoft.EntityFrameworkCore;
using ElectionEmployeeAPI.Models;

namespace ElectionEmployeeAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<AdminUser> AdminUsers { get; set; }
    

        public DbSet<PollingPersonnel> PollingPersonnel { get; set; }

        public DbSet<District> Districts { get; set; }
        public DbSet<Block> Blocks { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Block>()
                .HasKey(b => new { b.BlockId, b.DistrictId });

            modelBuilder.Entity<NewPartList>()
       .HasKey(x => new { x.District_ID, x.AC_No, x.Part_No });

            modelBuilder.Entity<NewPartList>()
                .ToTable("newpartlist");

            modelBuilder.Entity<PollingTeam>()
    .ToTable("polling_team");

            modelBuilder.Entity<PollingTeamMember>()
                .ToTable("polling_team_members");

            modelBuilder.Entity<AdminUser>()
        .ToTable("admin_users");


            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.UserId).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.MobileNumber).IsUnique();
            });

        }
        public DbSet<PWDTypeMaster> PWDTypeMaster { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Designation> Designations { get; set; }
        public DbSet<EmpType> EmpTypes { get; set; }
        public DbSet<AcList> AcList { get; set; }
        public DbSet<Bank> Banks { get; set; }
        public DbSet<Branch> Branches { get; set; }

        public DbSet<Office> Offices { get; set; }

        public DbSet<DesignationElectionDuty> DesignationElectionDuties { get; set; }
        public DbSet<DutyPost> DutyPosts { get; set; }
        public DbSet<ElectionWorkMaster> ElectionWorkMasters { get; set; }
        public DbSet<PollingTeam> PollingTeams { get; set; }
        public DbSet<PollingTeamMember> PollingTeamMembers { get; set; }
        public DbSet<NewPartList> NewPartLists { get; set; }
        public DbSet<ExemptionReason> ExemptionReasons { get; set; }
        public DbSet<DutyExemption> DutyExemptions { get; set; }
        public DbSet<UserAuditLog> UserAuditLogs { get; set; }


        public DbSet<User> Users { get; set; }
        public DbSet<OTP> OTPs { get; set; }
        public DbSet<AdminApproval> AdminApprovals { get; set; }
        public DbSet<PasswordResetRequest> PasswordResetRequests { get; set; }

        public DbSet<AdminDistrict> AdminDistricts { get; set; }
        public DbSet<DutyRemovalLog> DutyRemovalLogs { get; set; }





    }
}
