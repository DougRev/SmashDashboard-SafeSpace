using BusinessData;
using BusinessData.Interfaces;
using BusinessModels.Franchise;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace BusinessServices
{
    public class FranchiseService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserIdProvider _userIdProvider;

        public FranchiseService(ApplicationDbContext context, IUserIdProvider userIdProvider)
        {
            _context = context;
            _userIdProvider = userIdProvider;
        }


        public bool CreateFranchise(FranchiseCreate model)
        {
            var entity = new Franchise()
            {
                FranchiseId = model.FranchiseId,
                FranchiseName = model.FranchiseName,
                State = model.State,
                Owner1 = model.Owner1,
                Owner2 = model.Owner2,
                Owner3 = model.Owner3,
                Owner4 = model.Owner4,
                IsActive = true,
            };
            using (var ctx = new ApplicationDbContext())
            {
                ctx.Franchises.Add(entity);
                return ctx.SaveChanges() == 1;
            }
        }

        public IEnumerable<FranchiseListItem> GetFranchises()
        {
            using (var ctx = new ApplicationDbContext())
            {
                var query = ctx.Franchises
                    .Select(e => new FranchiseListItem
                    {
                        IsActive = e.IsActive,
                        FranchiseId = e.FranchiseId,
                        FranchiseName = e.FranchiseName,
                        State = e.State,
                        Status = e.Status,

                    });
                return query.ToArray();
            }
        }


        public FranchiseDetails GetClientsByFranchiseId(int franchiseId)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity = ctx.Franchises
                    .Include(f => f.Clients.Select(c => c.NationalAccount))  // Eagerly load the clients and their national accounts
                    .SingleOrDefault(f => f.FranchiseId == franchiseId);


                if (entity == null)
                {
                    return null;
                }

                // No need to query separately for the clients
                var clients = entity.Clients.ToList();

                return new FranchiseDetails
                {
                    FranchiseId = entity.FranchiseId,
                    FranchiseName = entity.FranchiseName,
                    Clients = clients,
                    State = entity.State,
                    IsActive = entity.IsActive,
                    Region = entity.Region,
                    LaunchDate = entity.LaunchDate,
                    BusinessAddress = entity.BusinessAddress,
                    BusinessCity = entity.BusinessCity,
                    BusinessState = entity.BusinessState,
                    BusinessZipCode = entity.BusinessZipCode,
                    BusinessPhone = entity.BusinessPhone,
                    Status = entity.Status,
                    Territories = entity.Territories,
                    Owner1 = entity.Owner1,
                    Owner1Email = entity.Owner1Email,
                    Owner1Phone = entity.Owner1Phone,
                    Owner2 = entity.Owner2,
                    Owner2Email = entity.Owner2Email,
                    Owner2Phone = entity.Owner2Phone,
                    Owner3 = entity.Owner3,
                    Owner3Email = entity.Owner3Email,
                    Owner3Phone = entity.Owner3Phone,
                    Owner4 = entity.Owner4,
                    Owner4Email = entity.Owner4Email,
                    Owner4Phone = entity.Owner4Phone,

                };
            }
        }


        public FranchiseDetails GetFranchiseById(int franchiseId)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity = ctx.Franchises.SingleOrDefault(e => e.FranchiseId == franchiseId);
                if (entity == null) return null;
                return
                    new FranchiseDetails
                    {
                        FranchiseId = entity.FranchiseId,
                        FranchiseName = entity.FranchiseName,
                        State = entity.State,
                        IsActive = entity.IsActive,
                        Region = entity.Region,
                        LaunchDate = entity.LaunchDate,
                        BusinessAddress = entity.BusinessAddress,
                        BusinessCity = entity.BusinessCity,
                        BusinessState = entity.BusinessState,
                        BusinessZipCode = entity.BusinessZipCode,
                        BusinessPhone = entity.BusinessPhone,
                        Status = entity.Status,
                        Territories = entity.Territories,
                        Owner1 = entity.Owner1,
                        Owner1Email = entity.Owner1Email,
                        Owner1Phone = entity.Owner1Phone,
                        Owner2 = entity.Owner2,
                        Owner2Email = entity.Owner2Email,
                        Owner2Phone = entity.Owner2Phone,
                        Owner3 = entity.Owner3,
                        Owner3Email = entity.Owner3Email,
                        Owner3Phone = entity.Owner3Phone,
                        Owner4 = entity.Owner4,
                        Owner4Email = entity.Owner4Email,
                        Owner4Phone = entity.Owner4Phone,
                    };
            }
        }


        public bool UpdateFranchise(FranchiseEdit model)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity =
                    ctx
                        .Franchises
                        .SingleOrDefault(e => e.FranchiseId == model.FranchiseId);

                // If the entity does not exist, return false.
                if (entity == null)
                {
                    return false;
                }

                // Update the entity's properties.
                entity.FranchiseName = model.FranchiseName;
                entity.State = model.State;
                entity.IsActive = model.IsActive;
                entity.Owner1 = model.Owner1;
                entity.Owner2 = model.Owner2;
                entity.Owner3 = model.Owner3;
                entity.Owner4 = model.Owner4;

                // Save the changes to the database.
                ctx.SaveChanges();

                // Return true.
                return true;
            }
        }

        public bool DeleteFranchise(int franchiseId)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity = ctx.Franchises.Single(e => e.FranchiseId == franchiseId);
                entity.IsActive = false;
                return ctx.SaveChanges() == 1;
            }
        }


        public double GetTotalCO2SavedForFranchiseById(int franchiseId)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var franchise = ctx.Franchises
                    .Include(f => f.Clients)
                    .SingleOrDefault(f => f.FranchiseId == franchiseId && f.IsActive);

                if (franchise == null) return 0;

                double totalCO2Saved = 0;
                foreach (var client in franchise.Clients)
                {
                    totalCO2Saved += client.TotalCO2SavedV2;
                }

                return totalCO2Saved;
            }
        }

        public int CountDistinctStatesWithClientsByFranchiseId(int franchiseId)
        {
            using (var ctx = new ApplicationDbContext())
            {
                int stateReach = 0;
                var franchise = ctx.Franchises
                                    .Include(f => f.Clients)
                                    .SingleOrDefault(f => f.FranchiseId == franchiseId);

                if (franchise != null)
                {
                    // Count distinct states of the clients within the franchise
                    return stateReach = franchise.Clients.Select(c => c.State).Distinct().Count();
                }

                return 0;
            }
        }

        public int CountClientsByFranchiseId(int franchiseId)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var franchise = ctx.Franchises
                                    .Include(f => f.Clients)
                                    .SingleOrDefault(f => f.FranchiseId == franchiseId);

                if (franchise != null)
                {
                    // Count the clients within the franchise
                    return franchise.Clients.Count;
                }

                return 0;
            }
        }

        public void SetFranchiseInactive(int franchiseId)
        {
            var franchise = _context.Franchises.SingleOrDefault(f => f.FranchiseId == franchiseId);

            if (franchise != null)
            {
                franchise.IsActive = false;
                _context.SaveChanges();
            }
        }


    }
}
