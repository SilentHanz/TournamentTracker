using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using TournamentReport.App_Start;
using TournamentReport.Models;

namespace TournamentReport.Controllers
{
    [Authorize(Roles = Constants.AdministratorsRoleName)]
    public class TournamentsController : Controller
    {
        private readonly TournamentContext db = new TournamentContext();

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Tournament tournament)
        {
            if (ModelState.IsValid)
            {
                db.Tournaments.Add(tournament);
                tournament.Owner = db.Users.First(u => u.Name == User.Identity.Name);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            return View(tournament);
        }

        public ActionResult Edit(int id)
        {
            Tournament tournament = db.Tournaments.Find(id);
            return View(tournament);
        }

        [HttpPost]
        public ActionResult Edit(Tournament tournament)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tournament).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return View(tournament);
        }

        public ActionResult Delete(int id)
        {
            Tournament tournament = db.Tournaments.Find(id);
            return View(tournament);
        }

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Tournament tournament = db.Tournaments.Find(id);
            db.Tournaments.Remove(tournament);
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}