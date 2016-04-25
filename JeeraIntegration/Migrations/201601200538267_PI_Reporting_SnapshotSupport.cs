namespace JiraIntegration.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PI_Reporting_SnapshotSupport : DbMigration
    {
        public override void Up()
        {
           AddColumn("dbo.Item", "SnapShotId", c => c.Long(nullable:true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Item", "SnapShotId");
        }
    }
}

//TODO: Add CreateTable Snapshot support rescaffold the migration and store the snapshot data to the snapshot table
//TODO: in the master return view add support for list
//TODO: In the master return view return items based on last snapshotid