using BusinessData;
using BusinessData.Enum;
using BusinessModels;
using BusinessServices;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.Sample.MVC4;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using Microsoft.AspNet.Identity;
using SelectPdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BusinessMVC2.Controllers
{
    public class ClientController : Controller
    {

        // GET: Clients
        public ActionResult Index()
        {
            //This Method will display races for a specific user.
            var userId = Guid.Parse(User.Identity.GetUserId());
            var service = new ClientService(userId);
            var model = service.GetBusinesses();

            return View(model);

            //return View();
        }


        public ActionResult Create()
        {
            var userId = Guid.Parse(User.Identity.GetUserId());
            var svc = new FranchiseService(userId);
            var svc2 = new NationalAccountService(userId);

            ViewBag.Franchises = svc.GetFranchises().Select(f => new
            {
                FranchiseId = f.FranchiseId,
                FranchiseName = f.FranchiseName,
            }).ToList();
            ViewBag.NationalAccounts = svc2.GetNationalAccounts().Select(f => new
            {
                AccountId = f.AccountId,
                AccountName = f.AccountName,
            });

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BusinessCreate model)
        {
            var userId = Guid.Parse(User.Identity.GetUserId());

            if (!ModelState.IsValid) // Check if the ModelState is NOT valid
            {
                var svc = new FranchiseService(userId);
                var svc2 = new NationalAccountService(userId);


                // Make sure the Franchises list is set correctly
                ViewBag.Franchises = svc.GetFranchises().Select(f => new
                {
                    FranchiseId = f.FranchiseId,
                    FranchiseName = f.FranchiseName
                }).ToList();
                ViewBag.NationalAccounts = svc2.GetNationalAccounts().Select(f => new
                {
                    AccountId = f.AccountId,
                    AccountName = f.AccountName,
                });

                return View(model); // Return the view with the same model when ModelState is invalid
            }

            // When the ModelState is valid, proceed with the creation process
            var service = new ClientService(userId);
            service.CreateBusiness(model);

            return RedirectToAction("Index");
        }



        [AllowAnonymous]
        public ActionResult Quote()
        {
            /*var svc = new FranchiseService();
            ViewBag.Franchises = svc.GetFranchises().Select(f => new
            {
                FranchiseId = f.FranchiseId,
                FranchiseName = f.FranchiseName
            }).ToList();*/

            return View();
        }


        [HttpPost]
        public ActionResult CalculateQuote(BusinessCreate quote)
        {
            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    System.Diagnostics.Debug.WriteLine($"Error: {error.ErrorMessage}");
                }
            }

            if (ModelState.IsValid)
            {
                // Create an instance of ClientService
                var service = new ClientService();

                // Perform calculations using the PerformCalculations method
                var calculationResults = service.PerformCalculations(quote);

                // Pass the calculation results to the Result view
                return View("Result", calculationResults);
            }
            else
            {
                var svc = new FranchiseService();
                ViewBag.Franchises = svc.GetFranchises().Select(f => new
                {
                    FranchiseId = f.FranchiseId,
                    FranchiseName = f.FranchiseName
                }).ToList();

                return View("Quote", quote);
            }
        }





        /*private double CalculateCO2Emission(double haulerEmissions, double smtEmissions)
        {
            double fuelConsumed = distance / (fuelEfficiency * 0.425143707); // Convert miles per gallon to liters per 100 km
            double co2Emission = fuelConsumed * emissionFactor;
            return co2Emission;
        }*/



        private ClientService CreateBusinessService()
        {
            var userId = Guid.Parse(User.Identity.GetUserId());
            var businessService = new ClientService(userId);
            return businessService;
        }


        //GET: Details
        //Client/Details/{id}
        public ActionResult Details(int id)
        {
            var svc = CreateBusinessService();
            var model = svc.GetBusinessById(id);
            return View(model);
        }


        //GET: Edit
        //Client/Edit/{id}
        public ActionResult Edit(int id)
        {
            var svc = CreateBusinessService();
            var detail = svc.GetBusinessById(id);
            var model = new BusinessEdit
            {
                BusinessId = detail.BusinessId,
                BusinessName = detail.BusinessName,
                FacilityID = detail.FacilityID,
                State = detail.State,
                City = detail.City,
                Address = detail.Address,
                ZipCode = detail.ZipCode,
                FranchiseId = detail.FranchiseId,
                Compactibility = detail.Compactibility,
                FranchiseeId = detail.FranchiseeId,
                ToClientDist = detail.ToClientDist,
                FromClientDist = detail.FromClientDist,
                ToHaulerDist = detail.ToHaulerDist,
                FromHaulerDist = detail.FromHaulerDist,
                LandfillDist = detail.LandfillDist,
                HaulsPerDay = detail.HaulsPerDay,
                NumberOfDumpsters = detail.NumberOfDumpsters,
            };

            // retrieve franchises and select the current franchise
            var userId = Guid.Parse(User.Identity.GetUserId());
            var franchiseService = new FranchiseService(userId);
            var franchises = franchiseService.GetFranchises();
            var selectedFranchise = franchises.FirstOrDefault(f => f.FranchiseId == detail.FranchiseId);
            model.Franchises = franchises.Select(f => new SelectListItem { Value = f.FranchiseId.ToString(), Text = f.FranchiseName, Selected = f == selectedFranchise }).ToList();

            return View(model);
        }


        //POST: Edit
        //Client/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, BusinessEdit model)
        {
            var currentUserId = User.Identity.GetUserId();
            //var ownerId = model.OwnerId;

            if (!ModelState.IsValid) return View(model);


            if (model.BusinessId != id)
            {
                ModelState.AddModelError("", "Id Mismatch");
                return View(model);
            }

            var service = CreateBusinessService();

            if (service.UpdateBusinesses(model))
            {
                TempData["SaveResult"] = "Your Client has been updated.";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Your Client could not be updated.");
            return View(model);
        }

        //GET: Delete
        //Client/Delete/{id}
        [ActionName("Delete")]
        public ActionResult Delete(int id)
        {
            var svc = CreateBusinessService();
            var model = svc.GetBusinessById(id);
            return View(model);
        }

        //POST: Delete
        //Client/Delete/{id}
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteBusiness(int id)
        {
            var service = CreateBusinessService();
            service.DeleteBusiness(id);
            TempData["SaveResult"] = "Your Client has been deleted.";

            return RedirectToAction("Index");
        }

        //Convert HTML to PDF
        /*public ActionResult BusinessDetailsToPdf(int id)
        {
            var userId = User.Identity.GetUserId();
            var svc = new ClientService(Guid.Parse(userId));
            var model = svc.GetBusinessById(id);

            // generate HTML code for the business details
            var htmlContent = this.RenderRazorViewToString("BusinessDetailsToPdf", model);

            ViewData.Add("TxtHtmlCode", htmlContent);

            return View(model);

        }*/

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SubmitAction(FormCollection collection)
        {
            // read parameters from the webpage
            string userId = collection["UserId"];
            Guid userGuid = Guid.Parse(userId);
            int businessId = Convert.ToInt32(collection["BusinessId"]);

            var svc = new ClientService(userGuid);
            var model = svc.GetBusinessById(businessId);

            // generate JavaScript to hide the navbar
            string script = "function hideNavbar(){var e=document.getElementsByClassName('navbar')[0];if(e){e.style.display='none';}}; hideNavbar();";

            // generate HTML code for the business details and inject JavaScript to hide the navbar
            string htmlString = RenderViewToString(ControllerContext, "~/Views/Client/BusinessDetailsToPdf.cshtml", model, script);


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

        public async Task UpdateGoogleSheet(Client client, int startRow)
        {
            string credentialsPath = Server.MapPath(ConfigurationManager.AppSettings["GoogleSheetsCredentialsPath"]);
            string tokenFolderPath = Server.MapPath(ConfigurationManager.AppSettings["GoogleSheetsTokenFolderPath"]);


            string[] Scopes = { SheetsService.Scope.Spreadsheets };
            string ApplicationName = "Smash-Dashboard";
            string sheetId = "1uyFl8JgzIzZ67lpFvE9et4cSOcziPrPA";
            string range = "Sheet1!A2"; // Adjust the range as needed

            // Read the JSON credentials file and create the SheetsService
            UserCredential credential;
            using (var stream = new FileStream(credentialsPath, FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(tokenFolderPath, true));
            }

            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Prepare the data to be appended
            var values = new List<IList<object>>
    {
        new List<object>
        {
            client.BusinessId,
            client.BusinessName,
            client.State,
            client.FacilityID,
            client.City,
            client.Address,
            client.ZipCode,
            client.OwnerId,
            client.FranchiseId,
            client.FranchiseName,
            client.FirstName,
            client.LastName,
            client.PhoneNumber,
            // Add more client properties as needed
        }
    };

            // Create a batch update request
            var requests = new List<Request>
    {
        new Request
        {
           UpdateCells = new UpdateCellsRequest
    {
        Start = new GridCoordinate
        {
            SheetId = 0, // Adjust the sheet ID as needed
            RowIndex = startRow, // Start from the given row
            ColumnIndex = 0 // Start from the first column
        },
        Rows = values.Select(row => new RowData { Values = row.Select(cell => new CellData { UserEnteredValue = new ExtendedValue { StringValue = cell?.ToString() ?? string.Empty } }).ToList() }).ToList(),
        Fields = "*"
    }
   }
  };

            // Execute the batch update request
            var batchUpdateRequest = new BatchUpdateSpreadsheetRequest { Requests = requests };
            var batchUpdateResponse = await service.Spreadsheets.BatchUpdate(batchUpdateRequest, sheetId).ExecuteAsync();
        }


        public async Task<ActionResult> TestUpdateGoogleSheet()
        {
            // Retrieve the list of clients from the database
            var db = new ApplicationDbContext();
            var clients = db.Clients.ToList();

            // Read the sheet data to find the last used row
            int lastUsedRow = await GetLastUsedRow();

            // Call UpdateGoogleSheet for each client, starting from the row after the last used row
            foreach (var client in clients)
            {
                lastUsedRow++;
                await UpdateGoogleSheet(client, lastUsedRow);
            }

            return RedirectToAction("Index");
        }

        public async Task<int> GetLastUsedRow()
        {
            string credentialsPath = Server.MapPath(ConfigurationManager.AppSettings["GoogleSheetsCredentialsPath"]);
            string tokenFolderPath = Server.MapPath(ConfigurationManager.AppSettings["GoogleSheetsTokenFolderPath"]);


            string[] Scopes = { SheetsService.Scope.Spreadsheets };
            string ApplicationName = "Smash-Dashboard";
            string sheetId = "1uyFl8JgzIzZ67lpFvE9et4cSOcziPrPA";
            string range = "Sheet1"; // Adjust the range as needed to cover the entire sheet

            // Read the JSON credentials file and create the SheetsService
            UserCredential credential;
            using (var stream = new FileStream(credentialsPath, FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(tokenFolderPath, true));
            }

            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Read the sheet data
            var response = await service.Spreadsheets.Values.Get(sheetId, range).ExecuteAsync();
            var values = response.Values;

            // Return the last used row index
            return values.Count - 1;
        }

        public async Task<ActionResult> ImportClientsFromGoogleSheet(CancellationToken cancellationToken)
        {
            var result = await new AuthorizationCodeMvcApp(this, new AppFlowMetadata()).AuthorizeAsync(cancellationToken);

            if (result.Credential != null)
            {
                await ImportClients(result.Credential);
                return RedirectToAction("Index"); // Redirect to the desired view after importing clients
            }
            else
            {
                return new RedirectResult(result.RedirectUri);
            }
        }

        private async Task ImportClients(UserCredential credential)
        {
            string ApplicationName = "Smash Calc";
            string sheetId = "17PA6YsX6PaCSQfHWYyNZmIvZp_WOMYBNtfa-7eZWldE";
            string range = "Sheet1!A2:Z";

            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            var response = await service.Spreadsheets.Values.Get(sheetId, range).ExecuteAsync();
            var values = response.Values;

            var clients = new List<Client>();

            using (var db = new ApplicationDbContext())
            {
                foreach (var row in values)
                {
                    if (row.Count >= 3 && int.TryParse(row[0].ToString(), out int franchiseId))
                    {
                        var client = new Client
                        {
                            FranchiseId = franchiseId,
                            BusinessName = row[1].ToString(),
                            FacilityID = row[2].ToString(),
                            Address = row[3].ToString(),
                            City = row[4].ToString(),
                            ZipCode = int.Parse(row[5].ToString()),
                            NumberOfDumpsters = int.Parse(row[6].ToString()),
                            HaulsPerDay = int.Parse(row[7].ToString()),
                            LandfillDist = int.Parse(row[8].ToString()),
                            State = (State)Enum.Parse(typeof(State), row[9].ToString(), true),
                            Compactibility = (Compactibility)Enum.Parse(typeof(Compactibility), row[10].ToString(), true),
                            AccountId = row.Count > 11 && int.TryParse(row[11].ToString(), out int accountId)
                                        && db.NationalAccounts.Any(na => na.AccountId == accountId)
                                        ? accountId
                                        : (int?)null,
                        };

                        clients.Add(client);
                    }
                    else
                    {
                        // Handle the row with an invalid FranchiseId value or missing columns
                    }
                }

                foreach (var client in clients)
                {
                    db.Clients.Add(client);
                }

                await db.SaveChangesAsync();
            }
        }




    }

}