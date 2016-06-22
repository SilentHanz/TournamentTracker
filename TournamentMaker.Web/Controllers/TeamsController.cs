using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using TournamentReport.App_Start;
using TournamentReport.Models;

namespace TournamentReport.Controllers
{
    [Authorize(Roles = Constants.AdministratorsRoleName)]
    public class TeamsController : Controller
    {
        readonly TournamentContext db = new TournamentContext();

        public ActionResult Create(string group, string tournamentSlug)
        {
            ViewBag.Group = group;
            ViewBag.TournamentSlug = tournamentSlug;
            return View();
        }

        [HttpPost]
        public ActionResult Create(Team team, string tournamentSlug)
        {
            if (ModelState.IsValid)
            {
                var tournament = db.Tournaments.First(t => t.Slug == tournamentSlug);
                team.Tournament = tournament;
                db.Teams.Add(team);
                db.SaveChanges();
                return RedirectToAction("Standings", "Home", new {tournamentSlug});
            }

            return View(team);
        }

        public ActionResult Edit(int id, string tournamentSlug)
        {
            var team = db.Teams.Include(t => t.Tournament).FirstOrDefault(t => t.Id == id);
            return View(team);
        }

        [HttpPost]
        public ActionResult Edit(Team team, string tournamentSlug)
        {
            if (ModelState.IsValid)
            {
                db.Entry(team).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Standings", "Home", new {id = tournamentSlug});
            }
            return View(team);
        }

        public ActionResult Delete(int id)
        {
            var team = db.Teams.Include(t => t.Tournament).FirstOrDefault(t => t.Id == id);
            return View(team);
        }

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var team = db.Teams.Include(t => t.Tournament).FirstOrDefault(t => t.Id == id);
            if (team == null) return HttpNotFound();
            string tournamentSlug = team.Tournament.Slug;
            db.Teams.Remove(team);
            db.SaveChanges();
            return RedirectToAction("Standings", "Home", new { id = tournamentSlug });
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}