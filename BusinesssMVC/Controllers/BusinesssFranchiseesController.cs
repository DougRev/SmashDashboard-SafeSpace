using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BusinesssMVC.Models;

namespace BusinesssMVC.Controllers
{
    public class BusinesssFranchiseesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BusinesssFranchisees
        public ActionResult Index()
        {
            var businesssFranchisees = db.BusinesssFranchisees.Include(b => b.Business);
            return View(businesssFranchisees.ToList());
        }

        // GET: BusinesssFranchisees/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BusinesssFranchisee businesssFranchisee = db.BusinesssFranchisees.Find(id);
            if (businesssFranchisee == null)
            {
                return HttpNotFound();
            }
            return View(businesssFranchisee);
        }

        // GET: BusinesssFranchisees/Create
        public ActionResult Create()
        {
            ViewBag.BusinessId = new SelectList(db.Businesses, "BusinessId", "BusinessName");
            return View();
        }

        // POST: BusinesssFranchisees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FranchiseeId,BusinessId")] BusinesssFranchisee businesssFranchisee)
        {
            if (ModelState.IsValid)
            {
                db.BusinesssFranchisees.Add(businesssFranchisee);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BusinessId = new SelectList(db.Businesses, "BusinessId", "BusinessName", businesssFranchisee.BusinessId);
            return View(businesssFranchisee);
        }

        // GET: BusinesssFranchisees/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BusinesssFranchisee businesssFranchisee = db.BusinesssFranchisees.Find(id);
            if (businesssFranchisee == null)
            {
                return HttpNotFound();
            }
            ViewBag.BusinessId = new SelectList(db.Businesses, "BusinessId", "BusinessName", businesssFranchisee.BusinessId);
            return View(businesssFranchisee);
        }

        // POST: BusinesssFranchisees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FranchiseeId,BusinessId")] BusinesssFranchisee businesssFranchisee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(businesssFranchisee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BusinessId = new SelectList(db.Businesses, "BusinessId", "BusinessName", businesssFranchisee.BusinessId);
            return View(businesssFranchisee);
        }

        // GET: BusinesssFranchisees/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BusinesssFranchisee businesssFranchisee = db.BusinesssFranchisees.Find(id);
            if (businesssFranchisee == null)
            {
                return HttpNotFound();
            }
            return View(businesssFranchisee);
        }

        // POST: BusinesssFranchisees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BusinesssFranchisee businesssFranchisee = db.BusinesssFranchisees.Find(id);
            db.BusinesssFranchisees.Remove(businesssFranchisee);
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
