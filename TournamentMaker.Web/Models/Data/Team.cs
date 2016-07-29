using System;
using System.Collections.Generic;
using System.Linq;

namespace TournamentReport.Models {
    public class Team {
        private bool _standingsCalculated;
        public int Id { get; set; }
        public virtual ICollection<Game> Games { get; set; }
        public string Group { get; set; }
        public Tournament Tournament { get; set; }

        public string Name { get; set; }
        public int Wins { get; private set; }
        public int Losses { get; private set; }
        public int Ties { get; private set; }

        public int GoalsScored { get; private set; }
        public int GoalsAgainst { get; private set; }
        public int GoalDifferential => GoalsScored - GoalsAgainst;

        public int GamesPlayed {
            get {
                if (Games == null) {
                    return 0;
                }
                return Games.Count(g => g.HomeTeamScore != null && g.InGame(this));
            }
        }

        public int ShutOuts
        {
            get
            {
                if (Games == null)
                {
                    return 0;
                }
                return Games.Where(g => g.HomeTeamId == Id).Count(g => g.AwayTeamScore == 0)
                    + Games.Where(g => g.AwayTeamId == Id).Count(g => g.HomeTeamScore == 0);
            }
        }

        public int Points
        {
            get
            {
                CalculateWinsLosses();
                return Wins * Tournament.PointsForWin
                    + Ties * Tournament.PointsForDraw
                    + Math.Min(GoalsScored * Tournament.PointsPerGoalScored, Tournament.MaxPointsForGoalsScored)
                    + ShutOuts * Tournament.PointsForShutOut;
            }
        }

        public int Cards {
            get {
                if (Games == null) {
                    return 0;
                }
                return Games.Where(g => g.HomeTeamId == Id).Sum(g => g.HomeTeamCards)
                    + Games.Where(g => g.AwayTeamId == Id).Sum(g => g.AwayTeamCards);
            }
        }

        private void CalculateWinsLosses() {
            if (Games == null) {
                return;
            }
            if (!_standingsCalculated) {
                Wins = 0;
                Losses = 0;
                Ties = 0;
                GoalsScored = 0;
                GoalsAgainst = 0;

                Func<Game, GameResult> gameResultDeterminator = game => {
                    if (game.HomeTeamScore == null) {
                        return null;
                    }
                    if (game.HomeTeam != null && game.HomeTeam.Id == Id)
                    {
                        return new GameResult(game.HomeTeamScore.Value, game.AwayTeamScore.GetValueOrDefault());
                    }
                    if (game.AwayTeam != null && game.AwayTeam.Id == Id)
                    {
                        return new GameResult(game.AwayTeamScore.GetValueOrDefault(), game.HomeTeamScore.Value);
                    }
                    return null;
                };

                foreach (var game in Games) {
                    var gameResult = gameResultDeterminator(game);

                    if (gameResult != null) {
                        if (gameResult.ThisTeamScore > gameResult.OtherTeamScore) {
                            Wins++;
                        }
                        if (gameResult.ThisTeamScore < gameResult.OtherTeamScore) {
                            Losses++;
                        }
                        if (gameResult.ThisTeamScore == gameResult.OtherTeamScore) {
                            Ties++;
                        }
                        GoalsScored += gameResult.ThisTeamScore;
                        GoalsAgainst += gameResult.OtherTeamScore;
                    }
                }
            }
            _standingsCalculated = true;
        }
    }
}