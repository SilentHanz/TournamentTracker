using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using TournamentReport.App_Start;
using TournamentReport.Models;

namespace TournamentReport.Controllers
{
    public class FieldsController : Controller
    {
        readonly TournamentContext db = new TournamentContext();

        // GET: Fields
        public ActionResult Index()
        {
            return View(db.Fields.ToList());
        }

        // GET: Fields/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var field = db.Fields.Find(id);
            if (field == null)
            {
                return HttpNotFound();
            }
            return View(field);
        }

        // GET: Fields/Create
        [Authorize(Roles = Constants.AdministratorsRoleName)]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Fields/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = Constants.AdministratorsRoleName)]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,Address")]Field field)
        {
            if (ModelState.IsValid)
            {
                db.Fields.Add(field);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(field);
        }

        // GET: Fields/Edit/5
        [Authorize(Roles = Constants.AdministratorsRoleName)]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var field = db.Fields.Find(id);
            if (field == null)
            {
                return HttpNotFound();
            }
            return View(field);
        }

        // POST: Fields/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constants.AdministratorsRoleName)]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,Address")]Field field)
        {
            if (ModelState.IsValid)
            {
                db.Entry(field).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(field);
        }

        // GET: Fields/Delete/5
        [Authorize(Roles = Constants.AdministratorsRoleName)]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var field = db.Fields.Find(id);
            if (field == null)
            {
                return HttpNotFound();
            }
            return View(field);
        }

        // POST: Fields/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constants.AdministratorsRoleName)]
        public ActionResult DeleteConfirmed(int id)
        {
            var field = db.Fields.Find(id);
            if (field == null)
            {
                return HttpNotFound();
            }
            db.Fields.Remove(field);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
