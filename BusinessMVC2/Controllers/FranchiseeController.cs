using BusinessModels;
using BusinessServices;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.ApplicationServices;
using System.Web.Mvc;

namespace BusinessMVC2.Controllers
{
    public class FranchiseeController : Controller
    {
        // GET: Franchisee
        public ActionResult Index()
        {
            //This Method will display races for a specific user.
            var userId = Guid.Parse(User.Identity.GetUserId());
            var service = new FranchiseeService(userId);
            var model = service.GetFranchisees();

            return View(model);

            //return View();
        }

        public ActionResult Create()
        {
            return View();
        }
        private FranchiseeService CreateFranchiseeService()
        {
            var userId = Guid.Parse(User.Identity.GetUserId());
            var franchiseeService = new FranchiseeService(userId);
            return franchiseeService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FranchiseeCreate model)
        {
            if (ModelState.IsValid)
            {
                return View(model);
            }
            var userId = Guid.Parse(User.Identity.GetUserId());
            var service = new FranchiseeService(userId);
            service.CreateFranchisee(model);

            return RedirectToAction("Index");
        }

        //GET: Details
        //Race/Details/{id}
        public ActionResult Details(int id)
        {
            var svc = CreateFranchiseeService();
            var model = svc.GetFranchiseeById(id);
            return View(model);
        }


        //GET: Edit
        //Race/Edit/{id}
        public ActionResult Edit(int id)
        {
            var svc = CreateFranchiseeService();
            var detail = svc.GetFranchiseeById(id);
            var model = new FranchiseeEdit
            {
                FranchiseeId = detail.FranchiseeId,
                FranchiseeName = detail.FranchiseeName,

            };
            return View(model);
        }

        //POST: Edit
        //Race/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, FranchiseeEdit model)
        {
            var currentUserId = User.Identity.GetUserId();
            //var ownerId = model.OwnerId;

            if (!ModelState.IsValid) return View(model);


            if (model.FranchiseeId != id)
            {
                ModelState.AddModelError("", "Id Mismatch");
                return View(model);
            }

            var service = CreateFranchiseeService();

            if (service.UpdateFranchisee(model))
            {
                TempData["SaveResult"] = "Your Franchisee has been updated.";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Your Franchisee could not be updated.");
            return View(model);
        }

        //GET: Delete
        //Franchisee/Delete/{id}
        [ActionName("Delete")]
        public ActionResult Delete(int id)
        {
            var svc = CreateFranchiseeService();
            var model = svc.GetFranchiseeById(id);
            return View(model);
        }

        //POST: Delete
        //Franchisee/Delete/{id}
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteFranchisee(int id)
        {
            var service = CreateFranchiseeService();
            service.DeleteFranchisee(id);
            TempData["SaveResult"] = "Your Franchisee has been deleted.";

            return RedirectToAction("Index");
        }
    }
}