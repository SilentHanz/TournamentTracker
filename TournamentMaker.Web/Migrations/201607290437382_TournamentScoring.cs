namespace TournamentReport.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TournamentScoring : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tournaments", "PointsForWin", c => c.Int(nullable: false, defaultValue: 3));
            AddColumn("dbo.Tournaments", "PointsForDraw", c => c.Int(nullable: false, defaultValue: 1));
            AddColumn("dbo.Tournaments", "PointsPerGoalScored", c => c.Int(nullable: false, defaultValue: 0));
            AddColumn("dbo.Tournaments", "MaxPointsForGoalsScored", c => c.Int(nullable: false, defaultValue: 3));
            AddColumn("dbo.Tournaments", "PointsForShutOut", c => c.Int(nullable: false, defaultValue: 0));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tournaments", "PointsForShutOut");
            DropColumn("dbo.Tournaments", "MaxPointsForGoalsScored");
            DropColumn("dbo.Tournaments", "PointsPerGoalScored");
            DropColumn("dbo.Tournaments", "PointsForDraw");
            DropColumn("dbo.Tournaments", "PointsForWin");
        }
    }
}
