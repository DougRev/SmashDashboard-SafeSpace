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
    public class FranchiseesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Franchisees
        public ActionResult Index()
        {
            return View(db.Franchisees.ToList());
        }

        // GET: Franchisees/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Franchisee franchisee = db.Franchisees.Find(id);
            if (franchisee == null)
            {
                return HttpNotFound();
            }
            return View(franchisee);
        }

        // GET: Franchisees/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Franchisees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FranchiseeId,FranchiseeName")] Franchisee franchisee)
        {
            if (ModelState.IsValid)
            {
                db.Franchisees.Add(franchisee);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(franchisee);
        }

        // GET: Franchisees/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Franchisee franchisee = db.Franchisees.Find(id);
            if (franchisee == null)
            {
                return HttpNotFound();
            }
            return View(franchisee);
        }

        // POST: Franchisees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FranchiseeId,FranchiseeName")] Franchisee franchisee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(franchisee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(franchisee);
        }

        // GET: Franchisees/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Franchisee franchisee = db.Franchisees.Find(id);
            if (franchisee == null)
            {
                return HttpNotFound();
            }
            return View(franchisee);
        }

        // POST: Franchisees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Franchisee franchisee = db.Franchisees.Find(id);
            db.Franchisees.Remove(franchisee);
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
