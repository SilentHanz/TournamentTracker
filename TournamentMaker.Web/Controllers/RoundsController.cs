using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using TournamentReport.App_Start;
using TournamentReport.Models;

namespace TournamentReport.Controllers
{
    public class RoundsController : Controller
    {
        readonly TournamentContext db = new TournamentContext();

        //
        // GET: /Rounds/

        public ViewResult Index()
        {
            var rounds = db.Rounds.Include(r => r.Tournament);
            return View(rounds.ToList());
        }

        //
        // GET: /Rounds/Details/5

        public ActionResult Details(int id)
        {
            var round = db.Rounds.Find(id);
            if (round == null) return HttpNotFound();

            return View(round);
        }

        //
        // GET: /Rounds/Create
        [Authorize(Roles = Constants.AdministratorsRoleName)]
        public ActionResult Create(string tournamentSlug)
        {
            var tournament = db.Tournaments.FirstOrDefault(t => t.Slug == tournamentSlug);
            if (tournament == null) return HttpNotFound();

            ViewBag.TournamentId = new SelectList(db.Tournaments, "Id", "Name", tournament.Id);
            return View();
        }

        //
        // POST: /Rounds/Create

        [HttpPost]
        [Authorize(Roles = Constants.AdministratorsRoleName)]
        public ActionResult Create(Round round, string tournamentSlug)
        {
            if (ModelState.IsValid)
            {
                db.Rounds.Add(round);
                db.SaveChanges();
                return RedirectToAction("Standings", "Home", new {tournamentSlug});
            }

            ViewBag.TournamentId = new SelectList(db.Tournaments, "Id", "Name", round.TournamentId);
            return View(round);
        }

        //
        // GET: /Rounds/Edit/5
        [Authorize(Roles = Constants.AdministratorsRoleName)]
        public ActionResult Edit(int id)
        {
            var round = db.Rounds.Find(id);
            if (round == null) return HttpNotFound();

            ViewBag.TournamentId = new SelectList(db.Tournaments, "Id", "Name", round.TournamentId);
            return View(round);
        }

        //
        // POST: /Rounds/Edit/5

        [HttpPost]
        [Authorize(Roles = Constants.AdministratorsRoleName)]
        public ActionResult Edit(Round round, string tournamentSlug)
        {
            if (ModelState.IsValid)
            {
                db.Entry(round).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Standings", "Home", new {tournamentSlug});
            }
            ViewBag.TournamentId = new SelectList(db.Tournaments, "Id", "Name", round.TournamentId);
            return View(round);
        }

        //
        // GET: /Rounds/Delete/5
        [Authorize(Roles = Constants.AdministratorsRoleName)]
        public ActionResult Delete(int id)
        {
            var round = db.Rounds.Find(id);
            if (round == null) return HttpNotFound();
            return View(round);
        }

        //
        // POST: /Rounds/Delete/5

        [HttpPost]
        [ActionName("Delete")]
        [Authorize(Roles = Constants.AdministratorsRoleName)]
        public ActionResult DeleteConfirmed(int id, string tournamentSlug)
        {
            var round = db.Rounds.Find(id);
            if (round == null) return HttpNotFound();

            db.Rounds.Remove(round);
            db.SaveChanges();
            return RedirectToAction("Standings", "Home", new {tournamentSlug});
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}