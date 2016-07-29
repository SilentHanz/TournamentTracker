using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TournamentReport.Models
{
    public class Tournament
    {
        public int Id { get; set; }
        public virtual ICollection<Round> Rounds { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Slug { get; set; }

        public string Description { get; set; }

        public virtual ICollection<Team> Teams { get; set; }

        public User Owner { get; set; }

        [Display(Name = "Points for a win")]
        public int PointsForWin { get; set; } = 3;

        [Display(Name = "Points for a draw")]
        public int PointsForDraw { get; set; } = 1;

        [Display(Name = "Points per goal scored")]
        public int PointsPerGoalScored { get; set; } = 0;

        /// <summary>
        /// If the tournament allows points for goals scored, this field
        /// allows you to cap it.
        /// </summary>
        [Display(Name = "Max points for goals scored")]
        public int MaxPointsForGoalsScored { get; set; } = 3;

        [Display(Name = "Points for shutout")]
        public int PointsForShutOut { get; set; } = 0;
    }
}