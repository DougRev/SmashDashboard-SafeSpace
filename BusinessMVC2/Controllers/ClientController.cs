using BusinessData;
using BusinessData.Enum;
using BusinessData.Interfaces;
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
using PagedList;
using SelectPdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
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

        // GET: Clients
        public ActionResult Index(int? page, string searchString)
        {
            int pageSize = 25;
            int pageNumber = (page ?? 1);
            var franchises = _context.Franchises.AsQueryable();

            string email = User.Identity.Name;

            var businessesQuery = _clientService.GetBusinesses().AsQueryable();

            if (email != "doug.revell@smashmytrash.com")
            {
                var names = email.Split('@')[0].Split('.');
                var formattedName = string.Join(" ", names.Select(name => char.ToUpper(name[0]) + name.Substring(1)));

                // Get all franchises owned by the currently logged-in user
                franchises = franchises.Where(f => f.Owner1.Trim() == formattedName ||
                                                       f.Owner2.Trim() == formattedName ||
                                                       f.Owner3.Trim() == formattedName ||
                                                       f.Owner4.Trim() == formattedName);

                // Get all client ids associated with these franchises
                var clientIds = franchises.SelectMany(f => f.Clients.Select(c => c.BusinessId)).ToList();

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

            return View(businesses.ToPagedList(pageNumber, pageSize));
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
                HaulsPerDay = detail.HaulsPerDay,
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

            var model = _clientService.GetBusinessById(businessId);

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
                await ImportClientsAndInvoices(result.Credential);
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
            string sheetId = "10JwDPXOTfCuGM8GUKkQJ4iDF4b3ksy6CEoxvr68lXSw";
            string range = "Clients!A1:Z";

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
                var franchises = await db.Franchises.ToListAsync();

                foreach (var row in values)
                {
                    if (row.Count >= 6 &&
                        !string.IsNullOrWhiteSpace(row[2].ToString()) &&
                        int.TryParse(row[2].ToString(), out int vonigoFranchiseId) &&
                        int.TryParse(row[4].ToString(), out int vonigoClientId) &&
                        !string.IsNullOrWhiteSpace(row[3].ToString()))
                    {
                        var franchiseName = row[1].ToString().Trim();
                        var franchise = franchises.SingleOrDefault(f => f.FranchiseName.Trim() == franchiseName);

                        if (franchise != null)
                        {
                            var client = new Client
                            {
                                FranchiseId = franchise.FranchiseId,
                                FranchiseName = franchiseName,
                                VonigoFranchiseId = vonigoFranchiseId,
                                BusinessName = row[3].ToString(),
                                VonigoClientId = vonigoClientId,
                                ServiceLocation = row[5].ToString(),
                            };

                            // Process the ServiceLocation to set Address, City, State, and ZipCode properties
                            ProcessLocationString(client.ServiceLocation, client);

                            clients.Add(client);
                        }
                    }
                }


                foreach (var client in clients)
                {
                    db.Clients.Add(client);
                }

                await db.SaveChangesAsync();
            }
        }

        private async Task ImportClientsAndInvoices(UserCredential credential)
        {
            string ApplicationName = "Smash Calc";
            string sheetId = "10JwDPXOTfCuGM8GUKkQJ4iDF4b3ksy6CEoxvr68lXSw";
            string range = "Clients!A2:Z";

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
                var franchises = await db.Franchises.ToListAsync();

                foreach (var row in values)
                {
                    if (row.Count >= 6 &&
                        !string.IsNullOrWhiteSpace(row[2].ToString()) &&
                        int.TryParse(row[2].ToString(), out int vonigoFranchiseId) &&
                        int.TryParse(row[4].ToString(), out int vonigoClientId) &&
                        !string.IsNullOrWhiteSpace(row[3].ToString()))
                    {
                        var franchiseName = row[1].ToString().Trim();
                        var franchise = franchises.SingleOrDefault(f => f.FranchiseName.Trim() == franchiseName);
                        var existingClients = await db.Clients.ToListAsync();


                        if (franchise != null)
                        {
                            var client = new Client
                            {
                                FranchiseId = franchise.FranchiseId,
                                FranchiseName = franchiseName,
                                VonigoFranchiseId = vonigoFranchiseId,
                                BusinessName = row[3].ToString(),
                                VonigoClientId = vonigoClientId,
                                ServiceLocation = row[5].ToString(),
                            };

                            // Process the ServiceLocation to set Address, City, State, and ZipCode properties
                            ProcessLocationString(client.ServiceLocation, client);

                            // Only add client if it does not exist yet
                            if (!existingClients.Any(c => c.BusinessName == client.BusinessName && c.ServiceLocation == client.ServiceLocation))
                            {
                                clients.Add(client);
                            }
                        }
                    }
                }

                foreach (var client in clients)
                {
                    db.Clients.Add(client);
                }

                await db.SaveChangesAsync();


                // Fetch the invoices
                range = "Invoices!A2:Z";
                response = await service.Spreadsheets.Values.Get(sheetId, range).ExecuteAsync();
                values = response.Values;

                var invoices = new List<Invoice>();
                var clientList = _clientService.GetBusinesses();


                foreach (var row in values)
                {
                    if (row.Count >= 11 &&
                        !string.IsNullOrWhiteSpace(row[0].ToString()) &&
                        int.TryParse(row[12].ToString(), out int vonigoInvoiceId) &&
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
                            };

                            // Only add client if it does not exist yet
                            if (!existingInvoices.Any(c => c.VonigoInvoiceId == invoice.VonigoInvoiceId))
                            {
                                invoices.Add(invoice);
                            }
                        }
                    }
                }

                // Import the invoices into the database
                foreach (var invoice in invoices)
                {
                    db.Invoices.Add(invoice);
                }

                db.SaveChanges();

                // Fetch the work orders
                range = "WorkOrders!A2:Z";
                response = await service.Spreadsheets.Values.Get(sheetId, range).ExecuteAsync();
                values = response.Values;

                var workOrders = new List<WorkOrder>();
                var invoiceList = await db.Invoices.ToListAsync(); // Using async version here


                foreach (var row in values)
                {

                    if (row.Count >= 22 &&
                        int.TryParse(row[0].ToString().Trim(), out int vonigoWorkOrderId) &&
                        int.TryParse(row[3].ToString(), out int vonigoFranchiseId) &&
                        int.TryParse(row[5].ToString(), out int vonigoClientId) &&
                        DateTime.TryParse(row[6].ToString(), out DateTime completedDate) &&
                        DateTime.TryParse(row[7].ToString(), out DateTime completedTime) &&
                        int.TryParse(row[8].ToString(), out int streetNumber) &&
                        bool.TryParse(row[13].ToString(), out bool vonigoIsActive) &&
                        int.TryParse(row[14].ToString(), out int dumpstersSmashed) &&
                        int.TryParse(row[19].ToString(), out int vonigoInvoiceId) &&
                        int.TryParse(row[20].ToString(), out int beforeFullness) &&
                        int.TryParse(row[21].ToString(), out int afterFullness))
                    {
                        var franchiseName = row[2].ToString().Trim();
                        var clientName = row[4].ToString().Trim();
                        var franchise = franchises.SingleOrDefault(f => f.FranchiseName.Trim() == franchiseName);
                        var invoice = invoiceList.SingleOrDefault(i => i.VonigoInvoiceId == vonigoInvoiceId);
                        var client = clientList.FirstOrDefault(c => c.BusinessName.Trim() == clientName);
                        var existingWorkOrders = await db.WorkOrders.ToListAsync();

                        if (franchise != null && invoice != null && client != null)
                        {
                            var workOrder = new WorkOrder
                            {
                                VonigoWorkOrderId = vonigoWorkOrderId,
                                InvoiceId = invoice.InvoiceId,
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
                                VonigoLabel = row[18].ToString(),
                                BeforeFullness = beforeFullness,
                                AfterFullness = afterFullness
                            };

                            // Only add Work Order if it does not exist yet
                            if (!existingWorkOrders.Any(c => c.VonigoWorkOrderId == workOrder.VonigoInvoiceId))
                            {
                                workOrders.Add(workOrder);
                            }
                        }

                    }
                }

                // Import the work orders into the database
                foreach (var workOrder in workOrders)
                {
                    db.WorkOrders.Add(workOrder);
                }

                await db.SaveChangesAsync();


                // Fetch the charges
                range = "Charges!A2:Z";
                response = await service.Spreadsheets.Values.Get(sheetId, range).ExecuteAsync();
                values = response.Values;

                var charges = new List<WOCharges>();
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
                        int.TryParse(row[12].ToString(), out int vonigoWorkOrderId) &&
                        int.TryParse(row[15].ToString(), out int vonigoFranchiseId))
                    {
                        var franchiseName = row[16].ToString().Trim();
                        var franchise = franchises.SingleOrDefault(f => f.FranchiseName.Trim() == franchiseName);
                        var workOrder = workOrderList.SingleOrDefault(w => w.VonigoWorkOrderId == vonigoWorkOrderId);
                        var invoice = await db.Invoices.ToListAsync(); // Using async version here
                        var existingCharges = await db.Charges.ToListAsync();

                        if (franchise != null && workOrder != null)
                        {
                            var woCharge = new WOCharges
                            {
                                VonigoChargeId = vonigoChargeId,
                                VonigoInvoiceId = workOrder.InvoiceId,
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
                                VonigoWorkOrderName = row[13].ToString(),
                                VonigoPriceListName = row[14].ToString(),
                                VonigoFranchiseId = vonigoFranchiseId,
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

                // Import the charges into the database
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



        public void ProcessLocationString(string location, Client client)
        {
            // Normalize the location string
            location = location.Replace("\n", " ").Replace("\r", "");

            // We'll break the string into words
            var words = location.Split(' ');

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

            if (int.TryParse(cityStateZipWords[cityStateZipWords.Count - 1].Trim(), out int zip))
            {
                client.ZipCode = zip; // This is the zip code: 30214, 28655, 85040
            }
            else
            {
                // Handle the case where the string is not a valid integer
            }

            // Extract City (everything else)
            client.City = string.Join(" ", cityStateZipWords.Take(cityStateZipWords.Count - 2)).Trim().TrimEnd(',');
        }

    }

}