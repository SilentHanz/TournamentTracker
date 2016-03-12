using System.Data.Entity.Migrations;

namespace TournamentReport.Migrations
{
    internal sealed class MigrationsConfiguration : DbMigrationsConfiguration<TournamentContext>
    {
        public MigrationsConfiguration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(TournamentContext context)
        {
            // This method will be called after migrating to the latest version.
            // Add or change data
            // context.SaveChanges();
        }
    }
}
