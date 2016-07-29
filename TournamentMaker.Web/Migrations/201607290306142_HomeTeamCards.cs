namespace TournamentReport.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HomeTeamCards : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Games", "HomeTeamCards", c => c.Int(nullable: false));
            AddColumn("dbo.Games", "AwayTeamCards", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Games", "AwayTeamCards");
            DropColumn("dbo.Games", "HomeTeamCards");
        }
    }
}
