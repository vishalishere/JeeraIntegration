namespace JiraIntegration.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PI_Reporting : DbMigration
    {
        public override void Up()
        {
            string sequence_snapshot = "CREATE SEQUENCE[dbo].[Sequence_Snapshot] AS[bigint] START WITH 1 INCREMENT BY 1 MINVALUE 1 MAXVALUE 9223372036854775807 CACHE";
            Sql(sequence_snapshot);

            CreateTable(
                "dbo.BaselineItem",
                c => new
                    {
                        Guid = c.String(nullable: false, maxLength: 128),
                        Title = c.String(),
                        Link = c.String(),
                        Project = c.String(),
                        Key = c.String(),
                        Summary = c.String(),
                        Description = c.String(),
                        StatusId = c.String(),
                        Status = c.String(),
                        Resolution = c.String(),
                        Epic = c.String(),
                        sprint = c.String(),
                        StoryPoints = c.String(),
                        AggregateTimeSpent = c.String(),
                        IssueType = c.String(),
                    })
                .PrimaryKey(t => t.Guid);
            
            CreateTable(
                "dbo.Item",
                c => new
                    {
                        Guid = c.String(nullable: false, maxLength: 128),
                        Title = c.String(),
                        Link = c.String(),
                        Project = c.String(),
                        Key = c.String(),
                        Summary = c.String(),
                        Description = c.String(),
                        StatusId = c.String(),
                        Status = c.String(),
                        Resolution = c.String(),
                        Epic = c.String(),
                        sprint = c.String(),
                        StoryPoints = c.String(),
                        AggregateTimeSpent = c.String(),
                        IssueType = c.String(),
                    })
                .PrimaryKey(t => t.Guid);
            
            CreateTable(
                "dbo.PlannedPointsForPI",
                c => new
                    {
                        Guid = c.String(nullable: false, maxLength: 128),
                        BaselinePlannedPointsForPI = c.String(),
                        TotalStoriesWithoutEstimates = c.String(),
                        TotalStories = c.String(),
                        TotalDefects = c.String(),
                    })
                .PrimaryKey(t => t.Guid);

            AddColumn("dbo.StoryDependency", "SOSStatus", c => c.String());
            AddColumn("dbo.StoryDependency", "SOSComment", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.StoryDependency", "SOSStatus");
            DropColumn("dbo.StoryDependency", "SOSComment");
            DropTable("dbo.PlannedPointsForPI");
            DropTable("dbo.Item");
            DropTable("dbo.BaselineItem");
            Sql("drop sequence[dbo].[Sequence_Snapshot]");
        }
    }
}
