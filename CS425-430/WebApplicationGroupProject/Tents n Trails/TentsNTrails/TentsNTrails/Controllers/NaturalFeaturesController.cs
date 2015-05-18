using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TentsNTrails.Models;

namespace TentsNTrails.Controllers
{
    public class NaturalFeaturesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// GET: NaturalFeatures 
        /// </summary>
        /// <returns>The NaturalFeatures/Index view.</returns>
        public ActionResult Index(string query)
        {
            List<NaturalFeature> features;
            if (!String.IsNullOrEmpty(query)) features = searchFor(query);
            else features = db.NaturalFeatures.ToList();
            return View(features);
        }

        /// <summary>
        /// Get a List of NaturalFeatures based on the searchQuery.
        /// </summary>
        /// <param name="query">The query to search for.  Defaults to "" if not set or null.</param>
        /// <returns>A list of all NaturalFeatures with a Name that starts with the search query.</returns>
        public List<NaturalFeature> searchFor(string query = "")
        {
            return db.NaturalFeatures
                .Where(f =>
                    f.Name.ToLower()
                    .StartsWith(query.ToLower())
                )
                .OrderBy(f => f.Name)
                .ToList();
        }

        /// <summary>
        /// <para>GET: NaturalFeatures/Details/5</para>
        /// <para>Shows the Details of a NaturalFeature by listing all associated Locations, with paging.</para>
        /// </summary>
        /// <param name="id">The ID of the NaturalFeature to view.</param>
        /// <param name="pageNumber">The page to view (defaults to 1)</param>
        /// <param name="pageSize">The page size (defaults to 10</param>
        /// <returns>The NaturalFeatures/Details view.</returns>
 
        public ActionResult Details(int? id, int pageNumber = 1, int pageSize = 10 )
        {
            // check parameters
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // check if valid Natural Feature.
            NaturalFeature naturalFeature = db.NaturalFeatures.Find(id);
            if (naturalFeature == null)
            {
                return HttpNotFound();
            }

            // create ViewModel, and populate with values.
            NaturalFeatureDetailsViewModel viewModel = new NaturalFeatureDetailsViewModel();
            viewModel.NaturalFeature = naturalFeature;
            viewModel.Locations = naturalFeature.LocationFeatures
                .Select(f => f.Location)
                .ToPagedList(pageNumber, pageSize);

            // done!
            return View(viewModel);
        }

        // GET: NaturalFeatures/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: NaturalFeatures/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name")] NaturalFeature naturalFeature)
        {
            if (ModelState.IsValid)
            {
                db.NaturalFeatures.Add(naturalFeature);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(naturalFeature);
        }

        // GET: NaturalFeatures/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NaturalFeature naturalFeature = db.NaturalFeatures.Find(id);
            if (naturalFeature == null)
            {
                return HttpNotFound();
            }
            return View(naturalFeature);
        }

        // POST: NaturalFeatures/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name")] NaturalFeature naturalFeature)
        {
            if (ModelState.IsValid)
            {
                db.Entry(naturalFeature).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(naturalFeature);
        }

        // GET: NaturalFeatures/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NaturalFeature naturalFeature = db.NaturalFeatures.Find(id);
            if (naturalFeature == null)
            {
                return HttpNotFound();
            }
            return View(naturalFeature);
        }

        // POST: NaturalFeatures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            NaturalFeature naturalFeature = db.NaturalFeatures.Find(id);
            db.NaturalFeatures.Remove(naturalFeature);
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
