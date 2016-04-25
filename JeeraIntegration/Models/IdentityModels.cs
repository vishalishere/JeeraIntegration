using JiraIntegration.Entities;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace JiraIntegration.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        // 
    }

    public class JiraDbContext : DbContext //  IdentityDbContext<ApplicationUser>
    {
        public JiraDbContext()
            : base("JiraIntegrationIdenityConnection")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<JiraDbContext, JiraIntegration.Migrations.Configuration>("JiraIntegrationIdenityConnection"));
        }

        public DbSet<Sprint> Sprint { get; set; }
        public DbSet<Team> Team { get; set; }
        public DbSet<Features> Feature { get; set; }
        public DbSet<MessageLog> MessageLog { get; set; }
        public DbSet<StoryDependency> StoryDependency { get; set; }
        public DbSet<Item> Item { get; set; }
        public DbSet<BaselineItem> BaselineItem { get; set; }
        public DbSet<PlannedPointsForPI> PlannedPointsForPI { get;set;}
        public DbSet<SnapShot> Snapshot { get; set; }
                
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }
    }
}