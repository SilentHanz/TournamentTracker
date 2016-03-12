using System.Data.Entity.Migrations;
using System.Web.DynamicData;
using DynamicDataEFCodeFirst;
using Microsoft.AspNet.DynamicData.ModelProviders;
using TournamentReport.App_Start;
using TournamentReport.Migrations;

[assembly: WebActivatorEx.PostApplicationStartMethod(typeof (AppInitializer), "PostStart")]

namespace TournamentReport.App_Start
{
    public class AppInitializer
    {
        public static void PostStart()
        {
            RunDatabaseMigrations();
            RegisterDynamicData();
        }

        private static void RunDatabaseMigrations()
        {
            var dbMigrator = new DbMigrator(new MigrationsConfiguration());
            dbMigrator.Update();
        }

        private static void RegisterDynamicData()
        {
            Registration.DefaultModel.RegisterContext(
                new EFDataModelProvider(() => new TournamentContext()),
                new ContextConfiguration {ScaffoldAllTables = true});
        }
    }
}