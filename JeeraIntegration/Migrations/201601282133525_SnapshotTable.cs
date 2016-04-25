namespace JiraIntegration.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SnapshotTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BaselineItem", "SnapShotId", c => c.Long(nullable: false));
            AddColumn("dbo.PlannedPointsForPI", "CurrentTotalPoints", c => c.String());
            AddColumn("dbo.PlannedPointsForPI", "TotalClosedPoints", c => c.String());
            AddColumn("dbo.PlannedPointsForPI", "SnapshotDate", c => c.String());
            AddColumn("dbo.PlannedPointsForPI", "SnapshotNumber", c => c.String());
                        
        }
        
        public override void Down()
        {
            DropColumn("dbo.PlannedPointsForPI", "SnapshotNumber");
            DropColumn("dbo.PlannedPointsForPI", "SnapshotDate");
            DropColumn("dbo.PlannedPointsForPI", "TotalClosedPoints");
            DropColumn("dbo.PlannedPointsForPI", "CurrentTotalPoints");
            DropColumn("dbo.BaselineItem", "SnapShotId");
            
        }
    }
}
