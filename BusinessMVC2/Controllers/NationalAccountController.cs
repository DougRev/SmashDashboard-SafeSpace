using BusinessData;
using BusinessModels.Franchise;
using BusinessModels;
using BusinessServices;
using Microsoft.AspNet.Identity;
using SelectPdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessModels.NationalAccount;
using BusinessData.Interfaces;

namespace BusinessMVC2.Controllers
{
    public class NationalAccountController : Controller
    {
        private readonly NationalAccountService _nationalAccountService;

        public NationalAccountController(NationalAccountService accountService)
        {
            _nationalAccountService = accountService;
        }


        // GET: NationalAccount
        public ActionResult Index()
        {
            var userId = Guid.Parse(User.Identity.GetUserId());
            var accounts = _nationalAccountService.GetNationalAccounts();

            var model = accounts.ToList().Select(f => new AccountListItem
            {
                AccountId = f.AccountId,
                AccountName = f.AccountName,
                // Add other properties as needed
            });

            return View(model);
        }


        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AccountCreate model)
        {
            if (ModelState.IsValid)
            {
                return View(model);
            }
            var userId = Guid.Parse(User.Identity.GetUserId());
            _nationalAccountService.CreateNationalAccount(model);

            return RedirectToAction("Index");
        }

        //GET: Details
        //NationalAccount/Details/{id}
        public ActionResult Details(int id)
        {
            var accountId = id;
            var model = _nationalAccountService.GetClientsByNationalAccountId(id);
            return View(model);
        }


        //GET: Edit
        //NationalAccount/Edit/{id}
        public ActionResult Edit(int id)
        {
            var detail = _nationalAccountService.GetNationalAccountById(id);
            var model = new AccountEdit
            {
                AccountId = detail.AccountId,
                AccountName = detail.AccountName,
                State = detail.State,
            };
            return View(model);
        }

        //POST: Edit
        //NationalAccount/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, AccountEdit model)
        {
            var currentUserId = User.Identity.GetUserId();
            if (!ModelState.IsValid) return View(model);


            if (model.AccountId != id)
            {
                ModelState.AddModelError("", "Id Mismatch");
                return View(model);
            }


            if (_nationalAccountService.UpdateNationalAccount(model))
            {
                TempData["SaveResult"] = "Your National Account has been updated.";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Your National Account could not be updated.");
            return View(model);
        }

        //GET: Delete
        //NationalAccount/Delete/{id}
        [ActionName("Delete")]
        public ActionResult Delete(int id)
        {
            var model = _nationalAccountService.GetNationalAccountById(id);
            return View(model);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteNationalAccount(int id)
        {

            // Update the IsActive field of the Franchise instead of deleting it
            _nationalAccountService.DeleteNationalAccount(id);

            TempData["SaveResult"] = "Your National Account has been marked as inactive.";

            return RedirectToAction("Index");
        }


        // GET: List
        // NationalAccount/List/{id}
        public ActionResult List(int id)
        {
            var clients = _nationalAccountService.GetClientsByNationalAccountId(id);
            var model = clients.Clients.Select(c => new BusinessListItem
            {
                BusinessId = c.BusinessId,
                BusinessName = c.BusinessName,
                FranchiseName = c.Franchise.FranchiseName,
                AccountName = c.AccountName,
                FacilityID = c.FacilityID,
                Address = c.Address,
                City = c.City,
                State = c.State,
                ZipCode = c.ZipCode,
                OwnerId = c.OwnerId,
            });

            // Retrieve the National Account name using the service method or any other way
            var nationalAccountName = _nationalAccountService.GetNationalAccountById(id)?.AccountName;

            // Pass the National Account name to the ViewBag
            ViewBag.NationalAccountName = nationalAccountName;

            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SubmitAction(FormCollection collection)
        {
            // read parameters from the webpage
            string userId = collection["UserId"];
            Guid userGuid = Guid.Parse(userId);
            int accountId = Convert.ToInt32(collection["AccountId"]);

            var account = _nationalAccountService.GetNationalAccountById(accountId);

            // Calculate the total CO2 saved for the franchise
            double totalCO2Saved = _nationalAccountService.GetTotalCO2SavedForFranchiseById(accountId);
            int states = _nationalAccountService.CountDistinctStatesWithClientsByFranchiseId(accountId);
            int locations = _nationalAccountService.CountClientsByFranchiseId(accountId);
            account.Locations = locations;
            account.TotalCO2Saved = totalCO2Saved;
            account.StateReach = states;

            // generate JavaScript to hide the navbar
            string script = "function hideNavbar(){var e=document.getElementsByClassName('navbar')[0];if(e){e.style.display='none';}}; hideNavbar();";

            // generate HTML code for the business details and inject JavaScript to hide the navbar
            string htmlString = RenderViewToString(ControllerContext, "~/Views/NationalAccount/AccountDetailsToPdf.cshtml", account, script);


            // instantiate a html to pdf converter object
            HtmlToPdf converter = new HtmlToPdf();

            string pdfPageSize = collection["DdlPageSize"];
            PdfPageSize pageSize = (PdfPageSize)Enum.Parse(typeof(PdfPageSize), pdfPageSize, true);

            string pdfOrientation = collection["DdlPageOrientation"];
            PdfPageOrientation pdfOrientationEnum = (PdfPageOrientation)Enum.Parse(typeof(PdfPageOrientation), pdfOrientation, true);

            int webPageWidth = 1024;
            try
            {
                webPageWidth = Convert.ToInt32(collection["TxtWidth"]);
            }
            catch
            {
            }

            int webPageHeight = 0;
            try
            {
                webPageHeight = Convert.ToInt32(collection["TxtHeight"]);
            }
            catch
            {
            }

            // set converter options
            converter.Options.PdfPageSize = pageSize;
            converter.Options.PdfPageOrientation = pdfOrientationEnum;
            converter.Options.WebPageWidth = webPageWidth;
            converter.Options.WebPageHeight = webPageHeight;

            // create a new pdf document converting an url
            PdfDocument doc = converter.ConvertHtmlString(htmlString, "https://businessmvc220230407125015.azurewebsites.net/");

            // save pdf document
            byte[] pdf = doc.Save();

            // close pdf document
            doc.Close();

            // return resulted pdf document
            FileResult fileResult = new FileContentResult(pdf, "application/pdf");
            fileResult.FileDownloadName = "Document.pdf";

            // Pass the total CO2 saved value to the viewbag to be displayed in the PDF
            ViewBag.TotalCO2Saved = totalCO2Saved;

            return fileResult;
        }




        protected string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }


        // helper method to render view to string
        private string RenderViewToString(ControllerContext context, string viewName, object model, string script)
        {
            ViewEngineResult result = ViewEngines.Engines.FindView(context, viewName, null);
            if (result.View == null)
            {
                throw new FileNotFoundException("View cannot be found.");
            }

            string viewData = "";
            using (var sw = new StringWriter())
            {
                var viewContext = new ViewContext(context, result.View, new ViewDataDictionary(model), new TempDataDictionary(), sw);
                result.View.Render(viewContext, sw);

                // Add the JavaScript code to the end of the HTML body
                var html = sw.ToString();
                html = html.Replace("</body>", $"<script>{script}</script></body>");

                viewData = html;
            }

            return viewData;
        }

    }
}