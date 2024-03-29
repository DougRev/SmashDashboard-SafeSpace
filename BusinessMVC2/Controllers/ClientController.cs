﻿using BusinessData;
using BusinessData.Enum;
using BusinessData.Interfaces;
using BusinessModels;
using BusinessMVC2.Models;
using BusinessServices;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.Sample.MVC4;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using log4net;
using Microsoft.AspNet.Identity;
using SelectPdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace BusinessMVC2.Controllers
{

    public class ClientController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserIdProvider _userIdProvider;
        private readonly FranchiseService _franchiseService;
        private readonly NationalAccountService _nationalAccountService;
        private readonly ClientService _clientService;


        public ClientController(
            ApplicationDbContext context,
            IUserIdProvider userIdProvider,
            FranchiseService franchiseService,
            NationalAccountService nationalAccountService,
            ClientService clientService)
        {
            _context = context;
            _userIdProvider = userIdProvider;
            _franchiseService = franchiseService;
            _nationalAccountService = nationalAccountService;
            _clientService = clientService;
        }

        private static readonly ILog Log = LogManager.GetLogger(typeof(Client));

        // GET: Clients
        public ActionResult Index(string searchString)
        {
            //var franchises = _context.Franchises.Include(f => f.Clients).AsQueryable();
            var franchises = _context.Franchises.Include(f => f.Clients).Include(f => f.Roles).AsQueryable();

            // Temporarily hardcode the user's email for testing
            //string email = "susan.schlaack@smashmytrash.com"; // Replace with the actual user's email


            string email = User.Identity.Name;

            var businessesQuery = _clientService.GetBusinesses().AsQueryable();

            if (email != "doug.revell@smashmytrash.com")
            {
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
                var firstFormattedName = formattedName.Split(' ')[0];
                if (abbreviations.ContainsKey(firstFormattedName))
                {
                    formattedName = formattedName.Replace(firstFormattedName, abbreviations[firstFormattedName]);
                }

                // Fetch franchises based on owner names
                var ownerFranchises = franchises.Where(f => f.Owner1.Trim() == formattedName ||
                                                             f.Owner2.Trim() == formattedName ||
                                                             f.Owner3.Trim() == formattedName ||
                                                             f.Owner4.Trim() == formattedName);

                // Fetch franchises based on role emails
                var roleFranchises = franchises.Where(f => f.Roles.Any(role => role.Email.Trim().Equals(email, StringComparison.OrdinalIgnoreCase)));

                // Debugging: Print out the fetched franchises for inspection
                foreach (var franchise in roleFranchises)
                {
                    Console.WriteLine($"Franchise ID: {franchise.FranchiseId}, Franchise Name: {franchise.FranchiseName}");
                    foreach (var role in franchise.Roles)
                    {
                        Console.WriteLine($"-- Role: {role.Role}, Email: {role.Email}");
                    }
                }


                // Combine both lists, removing duplicates
                var combinedFranchises = ownerFranchises.Union(roleFranchises).ToList();
                // Debugging: Print out combined franchises
                Console.WriteLine("Combined Franchises:");
                foreach (var franchise in combinedFranchises)
                {
                    Console.WriteLine($"Franchise ID: {franchise.FranchiseId}, Franchise Name: {franchise.FranchiseName}");
                }

                // Get all client ids associated with these franchises
                var clientIds = combinedFranchises.SelectMany(f => f.Clients.Select(c => c.BusinessId)).ToList();

                // Filter businesses to only those owned by the franchises
                businessesQuery = businessesQuery.Where(b => clientIds.Contains(b.BusinessId));


            }

            if (!String.IsNullOrEmpty(searchString))
            {
                businessesQuery = businessesQuery.Where(b => b.BusinessName.Contains(searchString));
            }

            var businesses = businessesQuery
                .OrderBy(b => b.BusinessName)
                .ToList();

            return View(businesses);
        }



        public ActionResult Create()
        {
            ViewBag.Franchises = _franchiseService.GetFranchises().Select(f => new
            {
                FranchiseId = f.FranchiseId,
                FranchiseName = f.FranchiseName,
            }).ToList();

            ViewBag.NationalAccounts = _nationalAccountService.GetNationalAccounts().Select(f => new
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
            if (!ModelState.IsValid) // Check if the ModelState is NOT valid
            {
                // Make sure the Franchises list is set correctly
                ViewBag.Franchises = _franchiseService.GetFranchises().Select(f => new
                {
                    FranchiseId = f.FranchiseId,
                    FranchiseName = f.FranchiseName
                }).ToList();

                ViewBag.NationalAccounts = _nationalAccountService.GetNationalAccounts().Select(f => new
                {
                    AccountId = f.AccountId,
                    AccountName = f.AccountName,
                });

                return View(model); // Return the view with the same model when ModelState is invalid
            }

            // When the ModelState is valid, proceed with the creation process
            var userId = _userIdProvider.GetUserId(); // Get user id from the provider
            var clientService = new ClientService(_context, _userIdProvider); // Create the ClientService with context and user id provider
            clientService.CreateBusiness(model);

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

                // Perform calculations using the PerformCalculations method
                var calculationResults = _clientService.PerformCalculations(quote);

                // Pass the calculation results to the Result view
                return View("Result", calculationResults);
            }
            else
            {
                ViewBag.Franchises = _franchiseService.GetFranchises().Select(f => new
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



        //GET: Details
        //Client/Details/{id}
        public ActionResult Details(int id)
        {
            var model = _clientService.GetBusinessById(id);
            //model.Invoices = _context.Invoices.Where(i => i.ClientId == id).ToList();

            return View(model);
        }




        //GET: Edit
        //Client/Edit/{id}
        public ActionResult Edit(int id)
        {
            var detail = _clientService.GetBusinessById(id);
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
                HaulsPerWeek = detail.HaulsPerWeek,
                NumberOfDumpsters = detail.NumberOfDumpsters,
            };

            // retrieve franchises and select the current franchise
            var userId = Guid.Parse(User.Identity.GetUserId());
            var franchises = _franchiseService.GetFranchises();
            var selectedFranchise = franchises.FirstOrDefault(f => f.FranchiseId == detail.FranchiseId);
            model.Franchises = franchises.Select(f => new SelectListItem { Value = f.FranchiseId.ToString(), Text = f.FranchiseName, Selected = f == selectedFranchise }).ToList();
            var nationalAccounts = _nationalAccountService.GetNationalAccounts();
            ViewBag.NationalAccounts = nationalAccounts.Select(na => new SelectListItem
            {
                Value = na.AccountId.ToString(),
                Text = na.AccountName
            }).ToList();
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


            if (_clientService.UpdateBusinesses(model))
            {
                TempData["SaveResult"] = "Your Client has been updated.";
                return RedirectToAction("Details", new { id = id });
            }

            ModelState.AddModelError("", "Your Client could not be updated.");
            return View(model);
        }

        //GET: Delete
        //Client/Delete/{id}
        [ActionName("Delete")]
        public ActionResult Delete(int id)
        {
            var model = _clientService.GetBusinessById(id);
            return View(model);
        }

        //POST: Delete
        //Client/Delete/{id}
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteBusiness(int id)
        {
            _clientService.DeleteBusiness(id);
            TempData["SaveResult"] = "Your Client has been deleted.";

            return RedirectToAction("Index");
        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SubmitAction(FormCollection collection)
        {
            // read parameters from the webpage
            string userId = collection["UserId"];
            Guid userGuid = Guid.Parse(userId);
            int businessId = Convert.ToInt32(collection["BusinessId"]);

            var model = _clientService.GetBusinessById(businessId);

            // generate JavaScript to hide the navbar
            string script = "function hideNavbar(){var e=document.getElementsByClassName('navbar')[0];if(e){e.style.display='none';}}; hideNavbar();";

            // generate HTML code for the business details and inject JavaScript to hide the navbar
            //string htmlString = RenderViewToString(ControllerContext, "~/Views/Client/BusinessDetailsToPdf.cshtml", model, script);
            string htmlString = RenderViewToString(ControllerContext, "~/Views/Client/ClientPdf2.cshtml", model, script);


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
            converter.Options.PdfPageOrientation = PdfPageOrientation.Landscape;
            converter.Options.WebPageWidth = webPageWidth;
            converter.Options.WebPageHeight = webPageHeight;

            // create a new pdf document converting an url
            PdfDocument doc = converter.ConvertHtmlString(htmlString, "https://smashmytrash.com/");

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

        //public async Task<ActionResult> TestUpdateGoogleSheet()
        //{
        //    // Retrieve the list of clients from the database
        //    var db = new ApplicationDbContext();
        //    var clients = db.Clients.ToList();

        //    // Read the sheet data to find the last used row
        //    int lastUsedRow = await GetLastUsedRow();

        //    // Call UpdateGoogleSheet for each client, starting from the row after the last used row
        //    foreach (var client in clients)
        //    {
        //        lastUsedRow++;
        //        await UpdateGoogleSheet(client, lastUsedRow);
        //    }

        //    return RedirectToAction("Index");
        //}

        public async Task<int> GetLastUsedRow(UserCredential credential)
        {
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Smash-Dashboard",
            });

            string sheetId = "17PA6YsX6PaCSQfHWYyNZmIvZp_WOMYBNtfa-7eZWldE"; // Replace with your sheet ID
            string range = "Sheet2!A2:A"; // Adjust the range to start from A2 and include all cells in column A

            // Read the sheet data
            var response = await service.Spreadsheets.Values.Get(sheetId, range).ExecuteAsync();
            var values = response.Values;

            // Check if values is null and return 1 (for A2) if it is, otherwise return the last used row index
            return values == null ? 1 : values.Count + 1;  // +1 to get the next row index for insertion
        }


        private async Task<UserCredential> GetGoogleCredential(CancellationToken cancellationToken)
        {
            var result = await new AuthorizationCodeMvcApp(this, new AppFlowMetadata()).AuthorizeAsync(cancellationToken);
            return result.Credential;
        }

        public async Task UpdateGoogleSheet(Client client, int startRow, UserCredential credential)
        {
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Smash-Dashboard",
            });

            string sheetId = "17PA6YsX6PaCSQfHWYyNZmIvZp_WOMYBNtfa-7eZWldE"; // Replace with your sheet ID
            string range = "Sheet2!A2"; // Adjust the range as needed

            int? desiredSheetId = await FetchSheetIdForName("Sheet2", credential);
            if (!desiredSheetId.HasValue)
            {
                // Handle error: Could not fetch the SheetId for the given sheet name
                throw new Exception("Could not fetch the SheetId for the given sheet name.");
            }

            // Define headers
            var headers = new List<string>
            {
                "BusinessId",
                "BusinessName",
                "State",
                "FacilityID",
                "City",
                "Address",
                "ZipCode",
                "OwnerId",
                "FranchiseId",
                "FranchiseName",
                "FirstName",
                "LastName",
                "PhoneNumber",
                // Add more client property names as headers
            };

            // Prepare the data to be appended
            var values = new List<IList<object>>();

            // If we are starting from the first row, add the headers first
            if (startRow == 2)
            {
                values.Add(headers.Cast<object>().ToList());
            }

            values.Add(new List<object>
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
            });

            // Create a batch update request
            var requests = new List<Request>
            {
                new Request
                {
                    UpdateCells = new UpdateCellsRequest
                    {
                        Start = new GridCoordinate
                        {
                            SheetId = desiredSheetId.Value, // Use the correct SheetId for Sheet2
                            RowIndex = startRow - 1, // Start from the given row (adjust for zero-based index)
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



        public Location ProcessLocationString(string locationString, Client client)
        {
            // Normalize the location string
            locationString = locationString.Replace("\n", " ").Replace("\r", "");

            // We'll break the string into words
            var words = locationString.Split(' ');

            // The last three or four words will form the city, state, and zip
            var cityStateZipWords = words.Skip(Math.Max(0, words.Length - 4)).ToList();

            // The words before the city, state, and zip will form the address
            var addressWords = words.Take(words.Length - cityStateZipWords.Count).ToList();

            client.Address = string.Join(" ", addressWords).Trim();

            // Extract State and Zip (last two elements)
            if (Enum.TryParse(cityStateZipWords[cityStateZipWords.Count - 2].Trim(), out State stateValue))
            {
                client.State = stateValue; // This is the state: GA, NC, AZ
            }
            else
            {
                // Handle the case where the string is not a valid member of the enum
            }

            client.ZipCode = cityStateZipWords[cityStateZipWords.Count - 1].Trim();


            // Extract City (everything else)
            client.City = string.Join(" ", cityStateZipWords.Take(cityStateZipWords.Count - 2)).Trim().TrimEnd(',');

            // Create a new Location object and fill it
            var location = new Location
            {
                ClientId = client.BusinessId, // Assign the ClientId to the location
                Street = client.Address,
                City = client.City,
                Providence = client.State.ToString(),
                ZipCode = client.ZipCode,
            };

            return location;
        }


        private async Task<IList<IList<object>>> GetSheetValues(UserCredential credential, string range)
        {
            string ApplicationName = "Smash Calc";
            string sheetId = "10JwDPXOTfCuGM8GUKkQJ4iDF4b3ksy6CEoxvr68lXSw";

            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            var response = await service.Spreadsheets.Values.Get(sheetId, range).ExecuteAsync();
            return response.Values;
        }

        public async Task<ActionResult> ImportFromGoogleSheet(CancellationToken cancellationToken)
        {
            var result = await new AuthorizationCodeMvcApp(this, new AppFlowMetadata()).AuthorizeAsync(cancellationToken);

            if (result.Credential != null)
            {
                await ImportData(result.Credential);
                return RedirectToAction("Index");
            }
            else
            {
                return new RedirectResult(result.RedirectUri);
            }
        }

        private async Task ImportData(UserCredential credential)
        {
            await ImportClients(credential);
            //await ImportNationalAccounts(credential);
            //await ImportInvoices(credential);
            //await ImportWorkOrders(credential);
        }

        public async Task<ActionResult> ExportToGoogleSheet(CancellationToken cancellationToken)
        {
            var result = await new AuthorizationCodeMvcApp(this, new AppFlowMetadata()).AuthorizeAsync(cancellationToken);

            if (result.Credential != null)
            {
                await ExportData(result.Credential);
                return RedirectToAction("Index");
            }
            else
            {
                return new RedirectResult(result.RedirectUri);
            }
        }

        private async Task ExportData(UserCredential credential)
        {
            await ExportClients(credential);

        }


        private async Task ExportClients(UserCredential credential)
        {
            // Fetch clients from the local database
            List<Client> clientsToExport;
            using (var db = new ApplicationDbContext())
            {
                clientsToExport = await db.Clients.ToListAsync();
            }

            // Use the Google Sheets API to export clients
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Smash-Dashboard",
            });

            string sheetId = "17PA6YsX6PaCSQfHWYyNZmIvZp_WOMYBNtfa-7eZWldE"; // Replace with your sheet ID
            int startRow = await GetLastUsedRow(credential) + 1; // Get the last used row and start exporting from the next row

            foreach (var client in clientsToExport)
            {
                await UpdateGoogleSheet(client, startRow, credential);
                startRow++;
            }
        }

        private async Task ImportClients(UserCredential credential)
        {
            var values = await GetSheetValues(credential, "Clients!A2:K");

            using (var db = new ApplicationDbContext())
            {
                var franchises = await db.Franchises.ToListAsync();

                foreach (var row in values)
                {
                    if (row.Count >= 11 &&
                        !string.IsNullOrWhiteSpace(row[0].ToString()) &&
                        int.TryParse(row[1].ToString(), out int franchiseID) &&
                        !string.IsNullOrWhiteSpace(row[2].ToString()) &&
                        int.TryParse(row[3].ToString(), out int vonigoClientId) &&
                        !string.IsNullOrWhiteSpace(row[4].ToString()) &&
                        !string.IsNullOrWhiteSpace(row[6].ToString()) &&
                        !string.IsNullOrWhiteSpace(row[7].ToString()) &&
                        !string.IsNullOrWhiteSpace(row[8].ToString()) &&
                        !string.IsNullOrWhiteSpace(row[9].ToString()) &&
                        !string.IsNullOrWhiteSpace(row[10].ToString()))
                    {
                        var franchiseName = row[0].ToString().Trim();
                        var clientName = row[2].ToString().Trim();
                        var serviceLocation = row[4].ToString().Trim();
                        var streetNo = row[5].ToString().Trim();
                        var street = row[6].ToString().Trim();
                        var city = row[7].ToString().Trim();
                        var state = row[8].ToString().Trim();
                        var zip = row[9].ToString().Trim();
                        var contactName = row[10].ToString().Trim();
                        var existingClients = await db.Clients.ToListAsync();

                        var franchise = franchises.SingleOrDefault(f => f.FranchiseName.Trim() == franchiseName);

                        if (franchise != null)
                        {
                            var client = new Client
                            {
                                FranchiseId = franchise.FranchiseId,
                                FranchiseName = franchiseName,
                                VonigoFranchiseId = franchiseID,
                                BusinessName = clientName,
                                VonigoClientId = vonigoClientId,
                                ServiceLocation = serviceLocation,
                                StreetNumber = streetNo,
                                Address = street,
                                City = city,
                                State = (State)Enum.Parse(typeof(State), state, true),
                                ZipCode = zip,
                                ContactName = contactName,
                            };

                            // Only add client if it does not exist yet
                            if (!existingClients.Any(c => c.BusinessName == client.BusinessName && c.ServiceLocation == client.ServiceLocation))
                            {
                                db.Clients.Add(client);
                                await db.SaveChangesAsync(); // Save client first

                                Trace.WriteLine($"Client {client.BusinessName} at {client.ServiceLocation} added.");

                                // Process the ServiceLocation to set Address, City, State, and ZipCode properties
                                var location = ProcessLocationString(client.ServiceLocation, client);

                                // Add the created location to Locations table
                                if (location != null)
                                {
                                    db.Locations.Add(location);
                                    await db.SaveChangesAsync(); // Save location after client
                                }
                            }
                            else
                            {
                                Trace.WriteLine($"Client {client.BusinessName} at {client.ServiceLocation} already exists.");
                            }
                        }
                        else
                        {
                            Trace.WriteLine($"Franchise with name {franchiseName} does not exist.");
                        }
                    }
                }
            }
        }

        private async Task ImportNationalAccounts(UserCredential credential)
        {
            var values = await GetSheetValues(credential, "National Accounts!A2:K");

            using (var db = new ApplicationDbContext())
            {
                var franchises = await db.Franchises.ToListAsync();
                var accounts = await db.NationalAccounts.ToListAsync();

                foreach (var row in values)
                {
                    if (row.Count >= 8 &&
                        !string.IsNullOrWhiteSpace(row[0].ToString()) &&
                        int.TryParse(row[1].ToString(), out int franchiseID) &&
                        !string.IsNullOrWhiteSpace(row[2].ToString()) &&
                        int.TryParse(row[3].ToString(), out int accountId) &&
                        !string.IsNullOrWhiteSpace(row[4].ToString()) &&
                        !string.IsNullOrWhiteSpace(row[5].ToString()) &&
                        !string.IsNullOrWhiteSpace(row[6].ToString()) &&
                        !string.IsNullOrWhiteSpace(row[7].ToString()) &&
                        !string.IsNullOrWhiteSpace(row[8].ToString()))
                    {
                        var franchiseName = row[0].ToString().Trim();
                        var clientName = row[2].ToString().Trim();
                        var serviceLocation = row[4].ToString().Trim();
                        var street = row[5].ToString().Trim();
                        var city = row[6].ToString().Trim();
                        var state = row[7].ToString().Trim();
                        var zip = row[8].ToString().Trim();
                        var existingClients = await db.Clients.ToListAsync();

                        var franchise = franchises.SingleOrDefault(f => f.FranchiseName.Trim() == franchiseName);
                        var account = accounts.SingleOrDefault(a => a.AccountId == accountId);

                        if (franchise != null)
                        {
                            var client = new Client
                            {
                                FranchiseId = franchise.FranchiseId,
                                FranchiseName = franchiseName,
                                VonigoFranchiseId = franchiseID,
                                BusinessName = clientName,
                                //VonigoClientId = vonigoClientId,
                                ServiceLocation = serviceLocation,
                                //StreetNumber = streetNo,
                                Address = street,
                                City = city,
                                State = (State)Enum.Parse(typeof(State), state, true),
                                ZipCode = zip,
                                AccountId = account.AccountId,
                                //ContactName = contactName,
                            };

                            // Only add client if it does not exist yet
                            if (!existingClients.Any(c => c.BusinessName == client.BusinessName && c.ServiceLocation == client.ServiceLocation))
                            {
                                db.Clients.Add(client);
                                await db.SaveChangesAsync(); // Save client first

                                // Process the ServiceLocation to set Address, City, State, and ZipCode properties
                                var location = ProcessLocationString(client.ServiceLocation, client);

                                // Add the created location to Locations table
                                if (location != null)
                                {
                                    db.Locations.Add(location);
                                    await db.SaveChangesAsync(); // Save location after client
                                }
                            }
                        }
                    }
                }
            }
        }


        private async Task ImportInvoices(UserCredential credential)
        {
            var values = await GetSheetValues(credential, "Invoices!A2:Z");

            var invoices = new List<Invoice>();

            using (var db = new ApplicationDbContext())
            {
                var franchises = await db.Franchises.ToListAsync();
                var clientList = _clientService.GetBusinesses();

                foreach (var row in values)
                {
                    if (row.Count >= 11 &&
                        !string.IsNullOrWhiteSpace(row[0].ToString()) &&
                        decimal.TryParse(row[5].ToString(), out decimal total) &&
                        int.TryParse(row[1].ToString(), out int vonigoFranchiseId) &&
                        int.TryParse(row[3].ToString(), out int vonigoClientId))
                    {
                        var franchiseName = row[0].ToString().Trim();
                        var clientName = row[2].ToString().Trim();
                        var address = row[6].ToString().Trim();
                        var franchise = franchises.SingleOrDefault(f => f.FranchiseName.Trim() == franchiseName);
                        var client = clientList.FirstOrDefault(c => c.BusinessName.Trim() == clientName);
                        var existingInvoices = await db.Invoices.ToListAsync();
                        var type = row[13].ToString().Trim();

                        string vonigoInvoiceId = row[12].ToString().Trim();  // vonigoInvoiceId is now string

                        if (franchise != null && client != null)
                        {
                            string phoneStr = row[11].ToString();
                            phoneStr = Regex.Replace(phoneStr, @"[^0-9]", ""); // keep only numeric characters

                            var invoice = new Invoice
                            {
                                FranchiseId = franchise.FranchiseId,
                                ClientId = client.BusinessId,
                                VonigoFranchiseId = vonigoFranchiseId,
                                VonigoInvoiceId = vonigoInvoiceId,
                                AccountType = row[4].ToString(),
                                TotalCost = (float)total,
                                Address = row[6].ToString(),
                                Status = row[7].ToString(),
                                SmtContact = row[8].ToString(),
                                AccountContact = row[9].ToString(),
                                ContactEmail = row[10].ToString(),
                                ContactPhone = phoneStr,
                                VonigoClientId = vonigoClientId,
                                InvoiceType = type,
                            };

                            // Only add client if it does not exist yet
                            if (!existingInvoices.Any(c => c.VonigoInvoiceId == invoice.VonigoInvoiceId))
                            {
                                invoices.Add(invoice);
                            }
                        }
                    }
                }

                foreach (var invoice in invoices)
                {
                    db.Invoices.Add(invoice);
                }

                await db.SaveChangesAsync();
            }
        }

        private async Task ImportWorkOrders(UserCredential credential)
        {
            var values = await GetSheetValues(credential, "WorkOrders!A2:Z");

            var workOrders = new List<WorkOrder>();

            using (var db = new ApplicationDbContext())
            {
                var franchises = await db.Franchises.ToListAsync();
                var invoiceList = await db.Invoices.ToListAsync();
                var clientList = _clientService.GetBusinesses();
                var locationsList = await db.Locations.ToListAsync();

                foreach (var row in values)
                {
                    if (row.Count >= 18 &&
                        int.TryParse(row[0].ToString().Trim(), out int vonigoWorkOrderId) &&
                        int.TryParse(row[3].ToString(), out int vonigoFranchiseId) &&
                        int.TryParse(row[5].ToString(), out int vonigoClientId) &&
                        DateTime.TryParse(row[6].ToString(), out DateTime completedDate) &&
                        DateTime.TryParse(row[7].ToString(), out DateTime completedTime) &&
                        int.TryParse(row[8].ToString(), out int streetNumber) &&
                        bool.TryParse(row[13].ToString(), out bool vonigoIsActive) &&
                        int.TryParse(row[14].ToString(), out int dumpstersSmashed))
                    {
                        string vonigoInvoiceId = row[18].ToString().Trim();  // vonigoInvoiceId is now string

                        var franchiseName = row[2].ToString().Trim();
                        var clientName = row[4].ToString().Trim();
                        var franchise = franchises.SingleOrDefault(f => f.FranchiseName.Trim() == franchiseName);
                        var invoice = invoiceList.SingleOrDefault(i => i.VonigoInvoiceId == vonigoInvoiceId);
                        string clientNameProcessed = Regex.Replace(clientName, @"[^0-9a-zA-Z]+", " ").ToLower();
                        var client = clientList.FirstOrDefault(c =>
                        {
                            string businessNameProcessed = Regex.Replace(c.BusinessName, @"[^0-9a-zA-Z]+", " ").ToLower();
                            return clientNameProcessed.Split(' ').All(part => businessNameProcessed.Contains(part));
                        });


                        var existingWorkOrders = await db.WorkOrders.ToListAsync();

                        if (franchise != null && client != null)
                        {
                            var workOrder = new WorkOrder
                            {
                                VonigoWorkOrderId = vonigoWorkOrderId,
                                InvoiceId = invoice?.InvoiceId,
                                Title = row[1].ToString(),
                                FranchiseId = franchise.FranchiseId,
                                VonigoClientId = vonigoClientId,
                                VonigoInvoiceId = vonigoInvoiceId,
                                CompletedDate = completedDate,
                                CompletedTime = completedTime,
                                VonigoIsActive = vonigoIsActive,
                                StreetNumber = streetNumber,
                                StreetAddress = row[9].ToString(),
                                City = row[10].ToString(),
                                State = row[11].ToString(),
                                Zipcode = row[12].ToString(),
                                DumpstersSmashed = dumpstersSmashed,
                                Summary = row[15].ToString(),
                                OnSiteContact = row[16].ToString(),
                                ServiceType = row[17].ToString(),
                            };


                            // If Invoice is null and ServiceType is "Recurring Service"
                            if (invoice == null && workOrder.ServiceType == "Recurring Services")
                            {
                                var address = workOrder.StreetNumber + " " + workOrder.StreetAddress;
                                var location = locationsList.FirstOrDefault(l => l.Street == address && l.City == workOrder.City && l.Providence == workOrder.State && l.ZipCode == workOrder.Zipcode);
                                if (location != null)
                                {
                                    var trackedClient = await db.Clients.FirstOrDefaultAsync(c => c.BusinessId == location.ClientId);
                                    if (trackedClient != null)
                                    {
                                        trackedClient.NumberOfDumpsters = dumpstersSmashed;
                                    }
                                }

                            }

                            // Only add Work Order if it does not exist yet
                            if (!existingWorkOrders.Any(c => c.VonigoWorkOrderId == workOrder.VonigoWorkOrderId))
                            {
                                workOrders.Add(workOrder);
                            }
                        }
                    }
                }

                foreach (var workOrder in workOrders)
                {
                    db.WorkOrders.Add(workOrder);
                }

                await db.SaveChangesAsync();
            }
        }



        private async Task ImportCharges(UserCredential credential)
        {
            var values = await GetSheetValues(credential, "Charges!A2:Z");

            var charges = new List<WOCharges>();

            using (var db = new ApplicationDbContext())
            {
                var franchises = await db.Franchises.ToListAsync();
                var workOrderList = db.WorkOrders.ToList();

                foreach (var row in values)
                {
                    if (row.Count >= 14 &&
                       int.TryParse(row[0].ToString().Trim(), out int vonigoChargeId) &&
                       DateTime.TryParse(row[4].ToString(), out DateTime createdDate) &&
                       DateTime.TryParse(row[5].ToString(), out DateTime editedDate) &&
                       bool.TryParse(row[6].ToString(), out bool activeCan) &&
                       float.TryParse(row[7].ToString(), out float tax) &&
                       float.TryParse(row[8].ToString(), out float subTotal) &&
                       int.TryParse(row[9].ToString(), out int quantity) &&
                       int.TryParse(row[12].ToString(), out int vonigoWorkOrderId))// &&
                                                                                   //int.TryParse(row[15].ToString(), out int vonigoFranchiseId))
                    {
                        var franchiseName = row[13].ToString().Trim();
                        var franchise = franchises.SingleOrDefault(f => f.FranchiseName.Trim() == franchiseName);
                        var workOrder = workOrderList.SingleOrDefault(w => w.VonigoWorkOrderId == vonigoWorkOrderId);//This line is the current error because it returns NULL
                        var invoice = await db.Invoices.ToListAsync(); // Using async version here
                        var existingCharges = await db.Charges.ToListAsync();

                        if (franchise != null && workOrder != null)
                        {
                            var woCharge = new WOCharges
                            {
                                VonigoChargeId = vonigoChargeId,
                                VonigoInvoiceId = workOrder.VonigoInvoiceId,
                                ItemType = row[1].ToString(),
                                ChargeName = row[2].ToString(),
                                Title = row[3].ToString(),
                                CreatedDate = createdDate,
                                EditedDate = editedDate,
                                ActiveCan = activeCan,
                                Tax = tax,
                                SubTotal = subTotal,
                                Quantity = quantity,
                                Description = row[10].ToString(),
                                Charge = row[11].ToString(),
                                VonigoWorkOrderId = vonigoWorkOrderId,
                                //VonigoWorkOrderName = row[13].ToString(),
                                //VonigoPriceListName = row[14].ToString(),
                                //VonigoFranchiseId = vonigoFranchiseId,
                                FranchiseId = franchise.FranchiseId,
                                WorkOrderId = workOrder.WorkOrderId,
                                FranchiseName = franchise.FranchiseName,
                                Total = subTotal + tax,
                            };

                            // Only add charge if it does not exist yet
                            if (!existingCharges.Any(c => c.VonigoChargeId == woCharge.VonigoChargeId))
                            {
                                charges.Add(woCharge);
                            }
                        }
                    }
                }

                foreach (var charge in charges)
                {
                    db.Charges.Add(charge);
                }

                await db.SaveChangesAsync();

                // Calculate and update the total charges for each work order
                foreach (var workOrderId in charges.Select(c => c.WorkOrderId).Distinct())
                {
                    var workOrder = await db.WorkOrders.FindAsync(workOrderId);
                    workOrder.TotalCharges = await db.Charges
                        .Where(c => c.WorkOrderId == workOrderId)
                        .SumAsync(c => c.Total);
                }

                await db.SaveChangesAsync();
            }
        }

        public async Task<int?> FetchSheetIdForName(string desiredSheetName, UserCredential credential)
        {
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Smash-Dashboard",
            });

            string spreadsheetId = "17PA6YsX6PaCSQfHWYyNZmIvZp_WOMYBNtfa-7eZWldE";

            var spreadsheet = await service.Spreadsheets.Get(spreadsheetId).ExecuteAsync();
            var desiredSheet = spreadsheet.Sheets.FirstOrDefault(s => s.Properties.Title == desiredSheetName);

            return desiredSheet?.Properties.SheetId;
        }

        private static string RemoveSpecialCharacters(string str)
        {
            return Regex.Replace(str, "[^a-zA-Z0-9_. ]+", "", RegexOptions.Compiled);

        }

        public ActionResult SelectFranchise()
        {
            ViewBag.Franchises = _franchiseService.GetFranchises().Select(f => new SelectListItem
            {
                Value = f.FranchiseId.ToString(),
                Text = f.FranchiseName
            }).ToList();

            return View();
        }

        [HttpPost]
        public ActionResult ReviewClients(int franchiseId)
        {
            var franchiseDetails = _franchiseService.GetClientsByFranchiseId(franchiseId);
            if (franchiseDetails == null)
            {
                // Handle the case where franchiseDetails is null
            }

            // Assuming franchiseDetails.Clients is a collection of client entities
            var clientListItems = franchiseDetails.Clients.Select(client => new BusinessListItem
            {
                BusinessId = client.BusinessId,
                BusinessName = client.BusinessName,
                FranchiseId = client.FranchiseId,
                FranchiseName = client.FranchiseName,
                ServiceLocation = client.ServiceLocation
                // Map other necessary properties here
            }).ToList();

            return View(clientListItems);
        }
        public ActionResult FranchisesWithOutOfStateClients()
        {
            int threshold = 10; // Set this based on your criteria
            var franchises = _franchiseService.GetFranchisesWithOutOfStateClients(threshold);
            return View(franchises);
        }


        [HttpPost]
        public ActionResult DeleteClients(int[] clientIds)
        {
            foreach (var clientId in clientIds)
            {
                _clientService.DeleteBusiness(clientId); // Assuming such a method exists
            }

            TempData["Message"] = "Selected clients have been deleted.";
            return RedirectToAction("Index"); // Redirect to a suitable page after deletion
        }

        public ActionResult ViewOutOfStateClients(int franchiseId)
        {
            // Retrieve the franchise including its clients
            var franchise = _context.Franchises
                                    .Include(f => f.Clients)
                                    .FirstOrDefault(f => f.FranchiseId == franchiseId);

            if (franchise == null)
            {
                // Handle the case when the franchise is not found
                return HttpNotFound();
            }

            // Convert the business state to enum (adjust according to your logic)
            var franchiseState = _franchiseService.ConvertToEnum(franchise.BusinessState);

            // Get out-of-state clients
            var outOfStateClients = franchise.Clients
                                             .Where(c => c.State != franchiseState)
                                             .Select(c => new OutOfStateClientViewModel
                                             {
                                                 ClientId = c.BusinessId,
                                                 ClientName = c.BusinessName,
                                                 ServiceLocation = c.ServiceLocation,
                                                 // ... other properties you need
                                             })
                                             .ToList();

            return View(outOfStateClients);
        }




        [HttpPost]
        public ActionResult ProcessSelectedClients(int[] selectedClients)
        {
            // Logic to process selected clients
            return RedirectToAction("Index"); // Redirect as needed
        }

        [HttpPost]
        public ActionResult DeleteOutOfStateClients(int franchiseId)
        {
            // Logic to delete all out-of-state clients for the given franchiseId
            // ...

            TempData["SuccessMessage"] = "All out-of-state clients for the franchise have been deleted.";
            return RedirectToAction("FranchisesWithOutOfStateClients"); // Or redirect to the appropriate view
        }

        // GET: Client/Import
        public ActionResult Import()
        {
            try
            {
                // Define the relative path to the CSV file
                string relativeFilePath = "~/Content/Uploads/Acadiana_LA_Clients.csv";

                // Call the import method
                _clientService.ImportClientsFromCsv(relativeFilePath);

                ViewBag.Message = "Clients imported successfully.";
            }
            catch (Exception ex)
            {
                ViewBag.Message = "ERROR:" + ex.Message.ToString();
            }

            return View();
        }

        // GET: Certificate/ClientPdf2
        public ActionResult ClientPdf2()
        {
            var model = new BusinessDetails
            {
                // Populate your model as necessary
                BusinessName = "Addison County Solid Waste District",
                // ... other properties
            };

            return View(model);
        }

        // In ClientController.cs

        [HttpPost]
        public ActionResult AssignOrphanClients()
        {
            const int TargetFranchiseId = 152;
            bool result = _clientService.AssignOrphanClientsToFranchise(TargetFranchiseId);

            if (result)
            {
                TempData["SuccessMessage"] = "All orphan clients have been successfully assigned to the target franchise.";
            }
            else
            {
                TempData["ErrorMessage"] = "There was an error assigning orphan clients, or there were no orphan clients to assign.";
            }

            return RedirectToAction("Index"); // Redirect to the client list or a relevant page
        }




    }

}