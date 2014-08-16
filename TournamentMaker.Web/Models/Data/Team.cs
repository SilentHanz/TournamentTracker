﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace TournamentReport.Models
{
    public class Team
    {
        private bool _standingsCalculated = false;
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

        public int GamesPlayed
        {
            get
            {
                if (Games == null)
                {
                    return 0;
                }
                return Games.Count(g => g.HomeTeamScore != null && g.InGame(this));
            }
        }

        public int Points
        {
            get
            {
                CalculateWinsLosses();
                return Wins*3 + Ties;
            }
        }

        private void CalculateWinsLosses()
        {
            if (Games == null)
            {
                return;
            }
            if (!_standingsCalculated)
            {
                Wins = 0;
                Losses = 0;
                Ties = 0;
                GoalsScored = 0;
                GoalsAgainst = 0;

                Func<Game, GameResult> gameResultDeterminator = (game) =>
                {
                    if (game.HomeTeamScore == null)
                    {
                        return null;
                    }
                    if (game.HomeTeamId == Id)
                    {
                        return new GameResult(game.HomeTeamScore.Value, game.AwayTeamScore.GetValueOrDefault());
                    }
                    return game.AwayTeamId == Id
                        ? new GameResult(game.AwayTeamScore.GetValueOrDefault(), game.HomeTeamScore.Value)
                        : null;
                };

                foreach (var game in Games)
                {
                    var gameResult = gameResultDeterminator(game);

                    if (gameResult != null)
                    {
                        if (gameResult.ThisTeamScore > gameResult.OtherTeamScore)
                        {
                            Wins++;
                        }
                        if (gameResult.ThisTeamScore < gameResult.OtherTeamScore)
                        {
                            Losses++;
                        }
                        if (gameResult.ThisTeamScore == gameResult.OtherTeamScore)
                        {
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