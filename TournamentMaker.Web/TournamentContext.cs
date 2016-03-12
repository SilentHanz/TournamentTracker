using System.Data.Entity;
using TournamentReport.Models;

namespace TournamentReport
{
    public class TournamentContext : DbContext, ITournamentContext
    {
        public DbSet<Team> Teams { get; set; }

        public DbSet<Round> Rounds { get; set; }

        public DbSet<Game> Games { get; set; }

        public DbSet<Tournament> Tournaments { get; set; }

        public IDbSet<User> Users { get; set; }

        public DbSet<Field> Fields { get; set; }
    }
}