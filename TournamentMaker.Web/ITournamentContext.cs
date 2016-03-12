using System.Data.Entity;
using TournamentReport.Models;

namespace TournamentReport
{
    public interface ITournamentContext
    {
        IDbSet<User> Users { get; set; }
    }
}