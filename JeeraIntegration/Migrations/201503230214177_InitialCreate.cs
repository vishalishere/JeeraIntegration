namespace JiraIntegration.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Features",
                c => new
                {
                    FeatureId = c.Int(nullable: false, identity: true),
                    FeatureName = c.String(nullable: false, maxLength: 50),
                    CompleteBySprintId = c.Int(nullable: false),
                    FromTeamId = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.FeatureId)
                .ForeignKey("dbo.Sprint", t => t.CompleteBySprintId)
                .ForeignKey("dbo.Team", t => t.FromTeamId);

            CreateTable(
                "dbo.Sprint",
                c => new
                {
                    SprintId = c.Int(nullable: false, identity: true),
                    SprintName = c.String(nullable: false, maxLength: 100),
                })
                .PrimaryKey(t => t.SprintId);

            CreateTable(
                "dbo.Team",
                c => new
                {
                    TeamId = c.Int(nullable: false, identity: true),
                    TeamName = c.String(nullable: false, maxLength: 50),
                    IsStarTeam = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.TeamId);

            CreateTable(
                "dbo.MessageLog",
                c => new
                {
                    MessageLogId = c.Int(nullable: false, identity: true),
                    StoryDependencyId = c.Int(nullable: false),
                    Request = c.String(),
                    RequestTime = c.DateTime(nullable: false),
                    Response = c.String(),
                    ResponseTime = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.MessageLogId)
                .ForeignKey("dbo.StoryDependency", t => t.StoryDependencyId);

            CreateTable(
                "dbo.StoryDependency",
                c => new
                {
                    StoryDependencyId = c.Int(nullable: false, identity: true),
                    FromTeamId = c.Int(nullable: false),
                    ToTeamId = c.Int(nullable: false),
                    Description = c.String(nullable: false, maxLength: 1000),
                    Detail = c.String(nullable: false, maxLength: 1000),
                    NeedBySprintId = c.Int(nullable: false),
                    RequestedByUserId = c.String(nullable: false),
                    BlockedStory = c.String(nullable: false, maxLength: 100),
                    DependencyStory = c.String(maxLength: 100),
                    Status = c.Int(nullable: false),
                    RejectReason = c.String(),
                    OwnedBy = c.String(),
                    DateCreated = c.DateTime(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.StoryDependencyId)
                .ForeignKey("dbo.Team", t => t.FromTeamId)
                .ForeignKey("dbo.Sprint", t => t.NeedBySprintId);

            CreateTable(
               "dbo.Snapshot",
               c => new
               {
                   Guid = c.String(nullable: false, maxLength: 128),
                   SnapshotId = c.String(nullable: true),
                   SnapshotDate = c.DateTime(nullable: true),
               })
               .PrimaryKey(t => t.Guid);
        }

        public override void Down()
        {
            DropForeignKey("dbo.MessageLog", "StoryDependencyId", "dbo.StoryDependency");
            DropForeignKey("dbo.StoryDependency", "NeedBySprintId", "dbo.Sprint");
            DropForeignKey("dbo.StoryDependency", "FromTeamId", "dbo.Team");
            DropForeignKey("dbo.Features", "FromTeamId", "dbo.Team");
            DropForeignKey("dbo.Features", "CompleteBySprintId", "dbo.Sprint");
            DropTable("dbo.StoryDependency");
            DropTable("dbo.MessageLog");
            DropTable("dbo.Team");
            DropTable("dbo.Sprint");
            DropTable("dbo.Features");
            DropTable("dbo.Snapshot");
        }
    }
}
