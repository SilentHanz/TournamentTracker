using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace TournamentReport.Models
{
    public class Game
    {
        public int Id { get; set; }

        public ICollection<Team> Teams { get; set; }

        public Team HomeTeam
        {
            get
            {
                if (Teams == null)
                {
                    return null;
                }
                if (HomeTeamId == null)
                {
                    return Teams.FirstOrDefault();
                }
                return Teams.FirstOrDefault(t => t.Id == HomeTeamId.Value);
            }
        }

        public Team AwayTeam
        {
            get
            {
                if (Teams == null)
                {
                    return null;
                }
                if (AwayTeamId == null)
                {
                    return Teams.LastOrDefault();
                }
                return Teams.LastOrDefault(t => t.Id == AwayTeamId.Value);
            }
        }

        public void AddTeams(Team homeTeam, Team awayTeam)
        {
            if (Teams == null) return;
            Teams.Clear();
            Teams.Add(homeTeam);
            HomeTeamId = homeTeam.Id;
            Teams.Add(awayTeam);
            AwayTeamId = awayTeam.Id;
        }

        public int? HomeTeamId { get; set; }

        public int? AwayTeamId { get; set; }

        [Display(Name = "Home Team Score")]
        [Range(0, int.MaxValue, ErrorMessage = "Not sure how you score negative goals.")]
        public int? HomeTeamScore { get; set; }

        [Display(Name = "Away Team Score")]
        [Range(0, int.MaxValue, ErrorMessage = "Not sure how you score negative goals.")]
        public int? AwayTeamScore { get; set; }

        [Display(Name = "Home Team Cards")]
        [Range(0, int.MaxValue, ErrorMessage = "Not sure how you are penalized negative cards.")]
        public int HomeTeamCards { get; set; } = 0;

        [Display(Name = "Away Team Cards")]
        [Range(0, int.MaxValue, ErrorMessage = "Not sure how you are penalized negative cards.")]
        public int AwayTeamCards { get; set; } = 0;

        public Round Round { get; set; }

        public int RoundId { get; set; }

        public DateTime? GameTime { get; set; }

        public int? FieldId { get; set; }

        public Field Field { get; set; }

        public bool InGame(Team team)
        {
            return team.Id == HomeTeam.Id || team.Id == AwayTeam.Id;
        }
    }
}