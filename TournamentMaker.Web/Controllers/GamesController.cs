using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using TournamentReport.App_Start;
using TournamentReport.Models;

namespace TournamentReport.Controllers
{
    [Authorize(Roles = Constants.AdministratorsRoleName)]
    public class GamesController : Controller
    {
        readonly TournamentContext db = new TournamentContext();

        public ActionResult Create(string tournamentSlug, int id)
        {
            ViewBag.RoundId = new SelectList(db.Rounds, "Id", "Name", id);
            var teams = db.Teams.Where(t => t.Tournament.Slug == tournamentSlug);
            ViewBag.HomeTeamId = new SelectList(teams, "Id", "Name");
            ViewBag.AwayTeamId = new SelectList(teams, "Id", "Name");

            var fields = db.Fields.ToList();
            ViewBag.FieldId = new SelectList(fields, "Id", "Name");

            var model = new GameCreateModel
            {
                RoundId = id
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(GameCreateModel gameModel, string tournamentSlug)
        {
            if (gameModel.HomeTeamId == gameModel.AwayTeamId)
            {
                ModelState.AddModelError("AwayTeamId", "A team cannot play itself");
            }
            if (ModelState.IsValid)
            {
                var game = new Game
                {
                    RoundId = gameModel.RoundId.Value,
                    GameTime = gameModel.GameTime
                };
                db.Games.Add(game);
                db.SaveChanges();

                var model = new GameEditModel
                {
                    Id = game.Id,
                    HomeTeamId = gameModel.HomeTeamId,
                    AwayTeamId = gameModel.AwayTeamId,
                    GameTime = gameModel.GameTime,
                    FieldId = gameModel.FieldId
                };
                return Edit(model, tournamentSlug);
            }

            var teams = db.Teams.Where(t => t.Tournament.Slug == tournamentSlug);
            ViewBag.HomeTeamId = new SelectList(teams, "Id", "Name");
            ViewBag.AwayTeamId = new SelectList(teams, "Id", "Name");

            ViewBag.RoundId = new SelectList(db.Rounds, "Id", "Name", gameModel.RoundId);
            return View(gameModel);
        }

        public ActionResult ReportScores(int id, string tournamentSlug)
        {
            var game = db.Games.Include(g => g.Teams).FirstOrDefault(g => g.Id == id);
            return View(game);
        }

        [HttpPost]
        public ActionResult ReportScores(Game game, string tournamentSlug)
        {
            if (ModelState.IsValid)
            {
                // Since we're only changing the score, we need to load the full
                // game from the database.
                var dbGame = db.Games.Include(g => g.Teams).FirstOrDefault(g => g.Id == game.Id);
                dbGame.AwayTeamScore = game.AwayTeamScore;
                dbGame.HomeTeamScore = game.HomeTeamScore;
                db.SaveChanges();
                return RedirectToAction("Standings", "Home", new {tournamentSlug});
            }

            return View(game);
        }

        public ActionResult Edit(int id, string tournamentSlug)
        {
            var game = db.Games.Include(g => g.Teams).FirstOrDefault(g => g.Id == id);
            if (game == null) return HttpNotFound();

            var model = new GameEditModel
            {
                Id = game.Id
            };

            if (game.HomeTeam != null)
            {
                model.HomeTeamId = game.HomeTeam.Id;
            }
            if (game.AwayTeam != null)
            {
                model.AwayTeamId = game.AwayTeam.Id;
            }
            model.GameTime = game.GameTime;

            var teams = db.Teams.Where(t => t.Tournament.Slug == tournamentSlug);
            ViewBag.HomeTeamId = new SelectList(teams, "Id", "Name", model.HomeTeamId);
            ViewBag.AwayTeamId = new SelectList(teams, "Id", "Name", model.AwayTeamId);

            var fields = db.Fields.ToList();
            ViewBag.FieldId = new SelectList(fields, "Id", "Name", model.FieldId);

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(GameEditModel game, string tournamentSlug)
        {
            if (game.HomeTeamId == game.AwayTeamId)
            {
                ModelState.AddModelError("AwayTeamId", "A team cannot play itself");
            }

            if (ModelState.IsValid)
            {
                var dbGame = db.Games.Include(g => g.Teams).FirstOrDefault(g => g.Id == game.Id);
                if (dbGame == null) return HttpNotFound();

                dbGame.AddTeams(db.Teams.Find(game.HomeTeamId), db.Teams.Find(game.AwayTeamId));
                dbGame.GameTime = game.GameTime;
                dbGame.FieldId = game.FieldId;
                db.SaveChanges();
                return RedirectToAction("Standings", "Home", new {tournamentSlug});
            }
            var teams = db.Teams.Where(t => t.Tournament.Slug == tournamentSlug);
            ViewBag.HomeTeamId = new SelectList(teams, "Id", "Name", game.HomeTeamId);
            ViewBag.AwayTeamId = new SelectList(teams, "Id", "Name", game.AwayTeamId);

            return View(game);
        }

        public ActionResult Delete(int id)
        {
            var game = db.Games.Find(id);
            if (game == null) return HttpNotFound();
            return View(game);
        }

        //
        // POST: /Games/Delete/5

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id, string tournamentSlug)
        {
            var game = db.Games.Find(id);
            if (game == null) return HttpNotFound();
            db.Games.Remove(game);
            db.SaveChanges();
            return RedirectToAction("Standings", "Home", new {tournamentSlug});
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public ActionResult ResetScores(string tournamentSlug)
        {
            return View();
        }

        [HttpPost]
        public ActionResult ResetScores(string tournamentSlug, string submit)
        {
            var games = (from team in db.Teams.Include(t => t.Games).Include(t => t.Tournament)
                where team.Tournament.Slug == tournamentSlug
                select team).SelectMany(t => t.Games).ToList();

            foreach (var game in games)
            {
                game.HomeTeamScore = null;
                game.AwayTeamScore = null;
            }
            db.SaveChanges();

            return RedirectToAction("Standings", "Home", new {tournamentSlug});
        }
    }
}