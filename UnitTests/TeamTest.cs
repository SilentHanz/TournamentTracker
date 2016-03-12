using System.Collections.Generic;
using TournamentReport.Models;
using Xunit;

public class TeamTest
{
    [Fact]
    public void TieCountsAsOnePoint()
    {
        // arrange
        var homeTeam = new Team();
        var awayTeam = new Team();
        var games = new List<Game> {
                new Game {
                    Teams = new List<Team> { homeTeam, awayTeam },
                    HomeTeamScore = 1,
                    AwayTeamScore = 1
                }
            };
        homeTeam.Games = games;

        // act
        int points = homeTeam.Points;

        // assert
        Assert.Equal(1, points);
    }

    [Fact]
    public void WinCountsAsThreePoints()
    {
        // arrange
        var homeTeam = new Team();
        var awayTeam = new Team();
        var games = new List<Game> {
                new Game {
                    Teams = new List<Team> { homeTeam, awayTeam },
                    HomeTeamScore = 2,
                    AwayTeamScore = 1
                }
            };
        homeTeam.Games = games;

        // act
        int points = homeTeam.Points;

        // assert
        Assert.Equal(3, points);
    }

    [Fact]
    public void LossCountsAsZeroPoints()
    {
        // arrange
        var homeTeam = new Team();
        var awayTeam = new Team();
        var games = new List<Game> {
                new Game {
                    Teams = new List<Team> { homeTeam, awayTeam },
                    HomeTeamScore = 1,
                    AwayTeamScore = 2
                }
            };
        homeTeam.Games = games;

        // act
        int points = homeTeam.Points;

        // assert
        Assert.Equal(0, points);
    }
}
