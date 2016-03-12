using System.Collections.Generic;
using TournamentReport.Models;

namespace TournamentReport.Models
{
    public class User : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public virtual ICollection<Tournament> Tournaments { get; set; }
    }
}