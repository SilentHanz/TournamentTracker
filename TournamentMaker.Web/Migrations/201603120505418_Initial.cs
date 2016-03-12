namespace TournamentReport.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Fields",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 4000),
                        Description = c.String(maxLength: 4000),
                        Address = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Games",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        HomeTeamId = c.Int(),
                        AwayTeamId = c.Int(),
                        HomeTeamScore = c.Int(),
                        AwayTeamScore = c.Int(),
                        RoundId = c.Int(nullable: false),
                        GameTime = c.DateTime(),
                        FieldId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Fields", t => t.FieldId)
                .ForeignKey("dbo.Rounds", t => t.RoundId, cascadeDelete: true)
                .Index(t => t.RoundId)
                .Index(t => t.FieldId);
            
            CreateTable(
                "dbo.Rounds",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 4000),
                        TournamentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tournaments", t => t.TournamentId, cascadeDelete: true)
                .Index(t => t.TournamentId);
            
            CreateTable(
                "dbo.Tournaments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 4000),
                        Slug = c.String(nullable: false, maxLength: 4000),
                        Description = c.String(maxLength: 4000),
                        Owner_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.Owner_Id)
                .Index(t => t.Owner_Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 4000),
                        Email = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Teams",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Group = c.String(maxLength: 4000),
                        Name = c.String(maxLength: 4000),
                        Wins = c.Int(nullable: false),
                        Losses = c.Int(nullable: false),
                        Ties = c.Int(nullable: false),
                        GoalsScored = c.Int(nullable: false),
                        GoalsAgainst = c.Int(nullable: false),
                        Tournament_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tournaments", t => t.Tournament_Id)
                .Index(t => t.Tournament_Id);
            
            CreateTable(
                "dbo.TeamGames",
                c => new
                    {
                        Team_Id = c.Int(nullable: false),
                        Game_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Team_Id, t.Game_Id })
                .ForeignKey("dbo.Teams", t => t.Team_Id, cascadeDelete: true)
                .ForeignKey("dbo.Games", t => t.Game_Id, cascadeDelete: true)
                .Index(t => t.Team_Id)
                .Index(t => t.Game_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Teams", "Tournament_Id", "dbo.Tournaments");
            DropForeignKey("dbo.TeamGames", "Game_Id", "dbo.Games");
            DropForeignKey("dbo.TeamGames", "Team_Id", "dbo.Teams");
            DropForeignKey("dbo.Rounds", "TournamentId", "dbo.Tournaments");
            DropForeignKey("dbo.Tournaments", "Owner_Id", "dbo.Users");
            DropForeignKey("dbo.Games", "RoundId", "dbo.Rounds");
            DropForeignKey("dbo.Games", "FieldId", "dbo.Fields");
            DropIndex("dbo.TeamGames", new[] { "Game_Id" });
            DropIndex("dbo.TeamGames", new[] { "Team_Id" });
            DropIndex("dbo.Teams", new[] { "Tournament_Id" });
            DropIndex("dbo.Tournaments", new[] { "Owner_Id" });
            DropIndex("dbo.Rounds", new[] { "TournamentId" });
            DropIndex("dbo.Games", new[] { "FieldId" });
            DropIndex("dbo.Games", new[] { "RoundId" });
            DropTable("dbo.TeamGames");
            DropTable("dbo.Teams");
            DropTable("dbo.Users");
            DropTable("dbo.Tournaments");
            DropTable("dbo.Rounds");
            DropTable("dbo.Games");
            DropTable("dbo.Fields");
        }
    }
}
