using BusinessData;
using BusinessModels;
using BusinessModels.Franchise;
using BusinessMVC2.Models;
using BusinesssData;
using BusinessServices;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.Sample.MVC4;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using Microsoft.AspNet.Identity;
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
    public class FranchiseController : Controller
    {
        ApplicationDbContext _context;
        // GET: Franchise
        public ActionResult Index(bool showAll = false)
        {
            var userId = Guid.Parse(User.Identity.GetUserId());
            var service = new FranchiseService(userId);
            var franchises = service.GetFranchises();

            if (!showAll)
            {
                franchises = franchises.Where(f => f.IsActive);
            }

            var model = franchises.ToList().Select(f => new FranchiseListItem
            {
                FranchiseId = f.FranchiseId,
                FranchiseName = f.FranchiseName,
                Status = f.Status,
                IsActive = f.IsActive,
                // Add other properties as needed
            });

            return View(model);
        }




        public ActionResult Create()
        {
            return View();
        }

        private FranchiseService CreateFranchiseService()
        {
            var userId = Guid.Parse(User.Identity.GetUserId());
            var franchiseService = new FranchiseService(userId);
            return franchiseService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FranchiseCreate model)
        {
            if (ModelState.IsValid)
            {
                return View(model);
            }
            var userId = Guid.Parse(User.Identity.GetUserId());
            var service = new FranchiseService(userId);
            service.CreateFranchise(model);

            return RedirectToAction("Index");
        }

        //GET: Details
        //Franchise/Details/{id}
        public ActionResult Details(int id)
        {
            var svc = CreateFranchiseService();
            var model = svc.GetClientsByFranchiseId(id);
            return View(model);
        }


        //GET: Edit
        //Franchise/Edit/{id}
        public ActionResult Edit(int id)
        {
            var svc = CreateFranchiseService();
            var detail = svc.GetFranchiseById(id);
            var model = new FranchiseEdit
            {
                FranchiseId = detail.FranchiseId,
                FranchiseName = detail.FranchiseName,
                State = detail.State,
                IsActive = detail.IsActive,
            };
            return View(model);
        }

        //POST: Edit
        //Franchise/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, FranchiseEdit model)
        {
            var currentUserId = User.Identity.GetUserId();
            if (!ModelState.IsValid) return View(model);


            if (model.FranchiseId != id)
            {
                ModelState.AddModelError("", "Id Mismatch");
                return View(model);
            }

            var service = CreateFranchiseService();

            if (service.UpdateFranchise(model))
            {
                TempData["SaveResult"] = "Your Franchise has been updated.";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Your Franchise could not be updated.");
            return View(model);
        }

        //GET: Delete
        //Franchise/Delete/{id}
        [ActionName("Delete")]
        public ActionResult Delete(int id)
        {
            var svc = CreateFranchiseService();
            var model = svc.GetFranchiseById(id);
            return View(model);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteFranchise(int id)
        {
            var service = CreateFranchiseService();

            // Update the IsActive field of the Franchise instead of deleting it
            service.SetFranchiseInactive(id);

            TempData["SaveResult"] = "Your Franchise has been marked as inactive.";

            return RedirectToAction("Index");
        }

        //View to show Client list by FranchiseId
        // GET: List
        // Franchise/List/{id}
        public ActionResult List(int id)
        {
            var svc = CreateFranchiseService();
            var clients = svc.GetClientsByFranchiseId(id);
            var model = clients.Clients.Select(c => new BusinessListItem
            {
                BusinessId = c.BusinessId,
                BusinessName = c.BusinessName,
                FranchiseName = c.Franchise.FranchiseName,
                FacilityID = c.FacilityID,
                Address = c.Address,
                City = c.City,
                State = c.State,
                ZipCode = c.ZipCode,
                OwnerId = c.OwnerId,
            });
            return View(model);
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

        public ActionResult AddClient(int id)
        {
            var franchise = _context.Franchises.SingleOrDefault(f => f.FranchiseId == id);

            if (franchise == null)
                return HttpNotFound();

            var viewModel = new ClientFormViewModel
            {
                Franchise = franchise,
                Client = new Client()
            };

            return View("ClientForm", viewModel);
        }

        [HttpPost]
        public ActionResult AddClient(Client client)
        {
            if (!ModelState.IsValid)
            {
                var viewModel = new ClientFormViewModel
                {
                    Client = client,
                    Franchise = _context.Franchises.Single(f => f.FranchiseId == client.FranchiseId)
                };

                return View("ClientForm", viewModel);
            }

            _context.Clients.Add(client);
            _context.SaveChanges();

            return RedirectToAction("Index", "Franchise");
        }

        public ActionResult AddFranchiseOwner(int id)
        {
            var franchise = _context.Franchises.SingleOrDefault(f => f.FranchiseId == id);

            if (franchise == null)
                return HttpNotFound();

            var viewModel = new FranchiseOwnerFormViewModel
            {
                Franchise = franchise,
                FranchiseOwner = new FranchiseOwner()
            };

            return View("FranchiseOwnerForm", viewModel);
        }

        [HttpPost]
        public ActionResult AddFranchiseOwner(FranchiseOwner franchiseOwner)
        {
            if (!ModelState.IsValid)
            {
                var viewModel = new FranchiseOwnerFormViewModel
                {
                    FranchiseOwner = franchiseOwner,
                    Franchise = _context.Franchises.Single(f => f.FranchiseId == franchiseOwner.FranchiseId)
                };

                return View("FranchiseOwnerForm", viewModel);
            }

            _context.Franchisees.Add(franchiseOwner);
            _context.SaveChanges();

            return RedirectToAction("Index", "Franchise");
        }

        public async Task<ActionResult> ImportFranchisesFromGoogleSheets(CancellationToken cancellationToken)
        {
            var result = await new AuthorizationCodeMvcApp(this, new AppFlowMetadata()).AuthorizeAsync(cancellationToken);

            if (result.Credential != null)
            {
                await ImportOwnersIFX(result.Credential);
                return RedirectToAction("Index"); // Redirect to the desired view after importing clients
            }
            else
            {
                return new RedirectResult(result.RedirectUri);
            }
        }

        private async Task ImportOwnersIFX(UserCredential credential)
        {
            string ApplicationName = "Territory Info";
            string sheetId = "10JwDPXOTfCuGM8GUKkQJ4iDF4b3ksy6CEoxvr68lXSw";
            string range = "Sheet1!A4:Z";

            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            var response = await service.Spreadsheets.Values.Get(sheetId, range).ExecuteAsync();
            var values = response.Values;

            var franchises = new List<Franchise>();

            using (var db = new ApplicationDbContext())
            {
                foreach (var row in values)
                {
                    var franchiseName = row[0].ToString();

                    // Search for the existing franchise in the database
                    var franchise = db.Franchises
                        .FirstOrDefault(f => f.FranchiseName == franchiseName);

                    if (franchise == null)
                    {
                        // If the franchise does not exist, create a new one
                        franchise = new Franchise
                        {
                            FranchiseName = franchiseName,
                            Status = row[1].ToString(),
                            Region = row[2].ToString(),
                            Owner1 = row.Count > 3 ? row[3].ToString() : null,
                            Owner1Email = row.Count > 4 ? row[4].ToString() : null,
                            Owner1Phone = row.Count > 5 ? row[5].ToString() : null,
                            Owner2 = row.Count > 6 ? row[6].ToString() : null,
                            Owner3 = row.Count > 7 ? row[7].ToString() : null,
                            Owner4 = row.Count > 8 ? row[8].ToString() : null,
                            Territories = row.Count > 10 ? row[10].ToString() : null,
                            BusinessState = row.Count > 11 ? row[11].ToString() : null,
                        };
                        franchise.IsActive = true;
                        db.Franchises.Add(franchise);
                    }
                    else
                    {
                        // If the franchise exists, update it
                        franchise.Status = row[1].ToString();
                        franchise.Region = row[2].ToString();
                        franchise.Owner1 = row.Count > 3 ? row[3].ToString() : null;
                        franchise.Owner1Email = row.Count > 4 ? row[4].ToString() : null;
                        franchise.Owner1Phone = row.Count > 5 ? row[5].ToString() : null;
                        franchise.Owner2 = row.Count > 6 ? row[6].ToString() : null;
                        franchise.Owner3 = row.Count > 7 ? row[7].ToString() : null;
                        franchise.Owner4 = row.Count > 8 ? row[8].ToString() : null;
                        franchise.Territories = row.Count > 10 ? row[10].ToString() : null;
                        franchise.BusinessState = row.Count > 11 ? row[11].ToString() : null;
                        franchise.IsActive = true;
                    }
                }

                var changes = db.SaveChanges();

                if (changes > 0)
                {
                    Console.WriteLine($"{changes} records were updated.");
                }
                else
                {
                    Console.WriteLine("No changes were made.");
                }
            }
        }


        public async Task<ActionResult> TestUpdateIFXGoogleSheet()
        {
            // Retrieve the list of clients from the database
            var db = new ApplicationDbContext();
            var franchises = db.Franchises.ToList();

            // Read the sheet data to find the last used row
            int lastUsedRow = await GetLastUsedRow();

            // Call UpdateGoogleSheet for each franchises starting from the row after the last used row
            foreach (var franchise in franchises)
            {
                lastUsedRow++;
                await UpdateGoogleSheet(franchise, lastUsedRow);
            }

            return RedirectToAction("Index");
        }
        public async Task UpdateGoogleSheet(Franchise franchise, int startRow)
        {
            string credentialsPath = Server.MapPath(ConfigurationManager.AppSettings["GoogleSheetsCredentialsPath"]);
            string tokenFolderPath = Server.MapPath(ConfigurationManager.AppSettings["GoogleSheetsTokenFolderPath"]);


            string[] Scopes = { SheetsService.Scope.Spreadsheets };
            string ApplicationName = "Territory Info";
            string sheetId = "10JwDPXOTfCuGM8GUKkQJ4iDF4b3ksy6CEoxvr68lXSw";
            string range = "Sheet1!A4:Z";

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
                    franchise.FranchiseId,
                    franchise.FranchiseName,
                    franchise.Status,
                    franchise.Region,
                    franchise.Owner1,
                    franchise.Owner1Email,
                    franchise.Owner1Phone,
                    franchise.Owner2,
                    franchise.Owner3,
                    franchise.Owner4,
                    franchise.Territories,
                    franchise.BusinessState,
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
        public async Task<int> GetLastUsedRow()
        {
            string credentialsPath = Server.MapPath(ConfigurationManager.AppSettings["GoogleSheetsCredentialsPath"]);
            string tokenFolderPath = Server.MapPath(ConfigurationManager.AppSettings["GoogleSheetsTokenFolderPath"]);


            string[] Scopes = { SheetsService.Scope.Spreadsheets };
            string ApplicationName = "Territory Info";
            string sheetId = "10JwDPXOTfCuGM8GUKkQJ4iDF4b3ksy6CEoxvr68lXSw";
            string range = "Sheet1!A4:Z";

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

    }
}