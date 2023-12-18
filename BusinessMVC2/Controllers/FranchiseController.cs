using BusinessData;
using BusinessModels;
using BusinessModels.Franchise;
using BusinessServices;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.Sample.MVC4;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using Microsoft.AspNet.Identity;
using PagedList;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Data.Entity;


namespace BusinessMVC2.Controllers
{
    public class FranchiseController : Controller
    {
        private readonly FranchiseService _franchiseService;
        private readonly ApplicationDbContext _context;

        public FranchiseController(FranchiseService franchiseService, ApplicationDbContext context)
        {
            _franchiseService = franchiseService;
            _context = context;
        }

        // GET: Franchise
        public ActionResult Index(bool showAll = false)
        {
            var franchises = _context.Franchises.Include(f => f.Roles).AsQueryable();

            if (User.Identity.IsAuthenticated)
            {
                var email = User.Identity.Name;

                // Admin user
                if (email == "doug.revell@smashmytrash.com")
                {
                    if (!showAll)
                    {
                        franchises = franchises.Where(f => f.IsActive);
                    }
                }
                else if (email.EndsWith("@smashmytrash.com"))
                {
                    // User is a franchise owner or has a role in a franchise
                    var names = email.Split('@')[0].Split('.');
                    var formattedName = string.Join(" ", names.Select(name => char.ToUpper(name[0]) + name.Substring(1)));

                    // Abbreviation map
                    Dictionary<string, string> abbreviations = new Dictionary<string, string>()
                    {
                        { "Jonathan", "Jon" },
                        {"James","Jim" },
                        {"Matthew", "Matt" },
                        // add more abbreviations as needed
                    };

                    // Check if first name is in the abbreviation map, if so replace with full name
                    if (abbreviations.TryGetValue(formattedName.Split(' ')[0], out var fullName))
                    {
                        formattedName = formattedName.Replace(formattedName.Split(' ')[0], fullName);
                    }

                    // Fetch franchises based on owner names
                    var ownerFranchises = franchises.Where(f => f.Owner1.Trim() == formattedName ||
                                                                f.Owner2.Trim() == formattedName ||
                                                                f.Owner3.Trim() == formattedName ||
                                                                f.Owner4.Trim() == formattedName);

                    // Fetch franchises based on role emails
                    var roleFranchises = franchises.Where(f => f.Roles.Any(role => role.Email.Trim().Equals(email, StringComparison.OrdinalIgnoreCase)));

                    // Combine both lists, removing duplicates
                    franchises = ownerFranchises.Union(roleFranchises);
                }
            }
            else if (!showAll)
            {
                franchises = franchises.Where(f => f.IsActive);
            }

            var franchiseListItems = franchises.Select(f => new FranchiseListItem
            {
                FranchiseId = f.FranchiseId,
                FranchiseName = f.FranchiseName,
                Status = f.Status,
                IsActive = f.IsActive
            }).ToList();

            return View(franchiseListItems);
        }



        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FranchiseCreate model)
        {
            if (ModelState.IsValid)
            {
                return View(model);
            }

            _franchiseService.CreateFranchise(model);

            return RedirectToAction("Index");
        }

        // GET: Details
        // Franchise/Details/{id}
        public ActionResult Details(int id)
        {
            var model = _franchiseService.GetFranchiseById(id); // Using GetFranchiseById to fetch all details
            if (model == null)
            {
                // Handle the situation where the franchise is not found
                return HttpNotFound(); // or another appropriate action
            }
            return View(model);
        }


        // GET: Edit
        // Franchise/Edit/{id}
        public ActionResult Edit(int id)
        {
            var detail = _franchiseService.GetFranchiseById(id);
            if (detail == null)
            {
                // Handle the case where the franchise is not found
                return HttpNotFound();
            }

            var model = new FranchiseEdit
            {
                FranchiseId = detail.FranchiseId,
                FranchiseName = detail.FranchiseName,
                AccountId = detail.AccountId,
                State = detail.State,
                IsActive = detail.IsActive,
                Owner1 = detail.Owner1,
                Owner2 = detail.Owner2,
                Owner3 = detail.Owner3,
                Owner4 = detail.Owner4,
                Roles = detail.Roles ?? new List<FranchiseRoleModel>() // Ensure Roles is initialized
            };

            return View(model);
        }

        // POST: Edit
        // Franchise/Edit/{id}
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

            // Check and add the new role if provided
            if (model.NewRole != null)
            {
                var newRoleModel = new BusinessModels.Franchise.FranchiseRoleModel
                {
                    // Map the properties from model.NewRole to newRoleModel
                    Name = model.NewRole.Name,
                    Email = model.NewRole.Email,
                    Phone = model.NewRole.Phone,
                    Role = model.NewRole.Role
                };

                var addRoleSuccess = _franchiseService.AddRoleToFranchise(id, newRoleModel);
                if (!addRoleSuccess)
                {
                    ModelState.AddModelError("", "Could not add the new role.");
                    return View(model);
                }
                // Re-fetch the franchise details to get the latest state
                var updatedFranchiseDetails = _franchiseService.GetFranchiseById(id);
                model.FranchiseName = updatedFranchiseDetails.FranchiseName;
                model.IsActive = updatedFranchiseDetails.IsActive;
                model.Owner1 = updatedFranchiseDetails.Owner1;
                model.Owner1Email = updatedFranchiseDetails.Owner1Email;
                model.Owner2 = updatedFranchiseDetails.Owner2;
                model.Owner2Email = updatedFranchiseDetails.Owner2Email;
                model.Owner3 = updatedFranchiseDetails.Owner3;
                model.Owner3Email = updatedFranchiseDetails.Owner3Email;
                model.Owner4 = updatedFranchiseDetails.Owner4;
                model.Owner4Email = updatedFranchiseDetails.Owner4Email;
            }

            if (_franchiseService.UpdateFranchise(model))
            {
                TempData["SaveResult"] = "Your Franchise has been updated.";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Your Franchise could not be updated.");
            return View(model);
        }
        public ActionResult EditRole(int roleId)
        {
            var role = _franchiseService.GetRoleById(roleId); // Implement this method in your service
            if (role == null)
            {
                return HttpNotFound();
            }

            var model = new FranchiseRoleModel
            {
                FranchiseRoleId = role.FranchiseRoleId,
                Name = role.Name,
                Email = role.Email,
                Phone = role.Phone,
                Role = role.Role
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditRole(FranchiseRoleModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var updateSuccess = _franchiseService.UpdateRole(model.FranchiseRoleId, model);
            if (updateSuccess)
            {
                return RedirectToAction("Edit", new { id = model.FranchiseId });
            }

            ModelState.AddModelError("", "Unable to update the role. Please try again.");
            return View(model);
        }


        // GET: Franchise/DeleteRole/{roleId}
        public ActionResult DeleteRole(int roleId)
        {
            var deleteSuccess = _franchiseService.DeleteRole(roleId);
            if (!deleteSuccess)
            {
                // Handle the error scenario
            }

            // Redirect to a relevant page after deletion
            return RedirectToAction("Index"); // Adjust the redirection as needed
        }


        //GET: Delete
        //Franchise/Delete/{id}
        [ActionName("Delete")]
        public ActionResult Delete(int id)
        {
            var model = _franchiseService.GetFranchiseById(id);
            return View(model);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteFranchise(int id)
        {

            // Update the IsActive field of the Franchise instead of deleting it
            _franchiseService.SetFranchiseInactive(id);

            TempData["SaveResult"] = "Your Franchise has been marked as inactive.";

            return RedirectToAction("Index");
        }

        //View to show Client list by FranchiseId
        // GET: List
        // Franchise/List/{id}
        public ActionResult List(int id)
        {
            var clients = _franchiseService.GetClientsByFranchiseId(id);

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
                ServiceLocation = c.ServiceLocation,
            }).ToList();

            return View(model);
        }



        public ActionResult ListFranchises(int? page, int pageSize = 25)
        {
            int pageNumber = (page ?? 1);
            var franchises = _context.Franchises.OrderBy(f => f.FranchiseId);
            return View(franchises.ToPagedList(pageNumber, pageSize));
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
            string range = "Franchises!A4:Z";

            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            var response = await service.Spreadsheets.Values.Get(sheetId, range).ExecuteAsync();
            var values = response.Values;

            var franchises = new List<Franchise>();
            var changeDetails = new List<string>(); // List to track changes

            using (var db = new ApplicationDbContext())
            {
                foreach (var row in values)
                {
                    var franchiseName = row[0].ToString();
                    var franchise = db.Franchises.FirstOrDefault(f => f.FranchiseName == franchiseName);

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

                        changeDetails.Add($"Created new franchise: {franchiseName}");
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

                        changeDetails.Add($"Updated franchise: {franchiseName}");
                    }
                }

                db.SaveChanges();
            }

            if (changeDetails.Any())
            {
                TempData["Notification"] = $"Changes made:\n{string.Join("\n", changeDetails)}";
            }
            else
            {
                TempData["Notification"] = "No changes were made.";
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
            string range = "Franchises!A4:Z";

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
            string range = "Franchises!A4:Z";

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