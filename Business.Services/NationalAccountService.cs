using BusinessData;
using BusinessModels.Franchise;
using BusinessModels.NationalAccount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;
using BusinessData.Interfaces;

namespace BusinessServices
{
    public class NationalAccountService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserIdProvider _userIdProvider;

        public NationalAccountService(ApplicationDbContext context, IUserIdProvider userIdProvider)
        {
            _context = context;
            _userIdProvider = userIdProvider;
        }


        public bool CreateNationalAccount(AccountCreate model)
        {
            var entity = new NationalAccount()
            {
                OwnerId = _userIdProvider.GetUserId(), // Use GetUserId() to get the current user id
                AccountId = model.AccountId,
                AccountName = model.AccountName,
                State = model.State,
            };

            _context.NationalAccounts.Add(entity);
            return _context.SaveChanges() == 1;
        }


        public IEnumerable<AccountListItem> GetNationalAccounts()
        {
            using (var ctx = new ApplicationDbContext())
            {
                var query = ctx.NationalAccounts
                    .Select(e => new AccountListItem
                    {
                        AccountId = e.AccountId,
                        AccountName = e.AccountName,
                        State = e.State,


                    });
                return query.ToArray();
            }
        }

        public AccountDetails GetClientsByNationalAccountId(int accountId)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var account = ctx.NationalAccounts
                    .SingleOrDefault(f => f.AccountId == accountId);

                if (account == null)
                {
                    return null;
                }

                var clients = ctx.Clients
                    .Include(c => c.Franchise) // Make sure to include the Franchise navigation property
                                               // Include other necessary properties as needed
                    .Where(c => c.AccountId == accountId)
                    .ToList();

                return new AccountDetails
                {
                    AccountId = account.AccountId,
                    AccountName = account.AccountName,
                    Clients = clients
                };
            }
        }






        public AccountDetails GetNationalAccountById(int accountId)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity = ctx.NationalAccounts.SingleOrDefault(e => e.AccountId == accountId);
                if (entity == null) return null;
                return
                    new AccountDetails
                    {
                        AccountId = entity.AccountId,
                        AccountName = entity.AccountName,
                    };
            }
        }


        public bool UpdateNationalAccount(AccountEdit model)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity =
                    ctx
                        .NationalAccounts
                        .SingleOrDefault(e => e.AccountId == model.AccountId);

                // If the entity does not exist, return false.
                if (entity == null)
                {
                    return false;
                }

                // Update the entity's properties.
                entity.AccountName = model.AccountName;
                entity.State = model.State;
                // Save the changes to the database.
                ctx.SaveChanges();

                // Return true.
                return true;
            }
        }
        
        // OLD DELETE CHANGED 11/9/23

        //public bool DeleteNationalAccount(int accountId)
        //{
        //    using (var ctx = new ApplicationDbContext())
        //    {
        //        var entity = ctx.NationalAccounts.Single(e => e.AccountId == accountId);
        //        ctx.NationalAccounts.Remove(entity);
        //        return ctx.SaveChanges() == 1;
        //    }
        //}
        public bool DeleteNationalAccount(int accountId)
        {
            using (var ctx = new ApplicationDbContext())
            {
                // Start a transaction to ensure full rollback in case of failure
                using (var transaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        // Load the NationalAccount including its related Clients
                        var nationalAccount = ctx.NationalAccounts
                                                 .Include(na => na.Clients.Select(c => c.Locations)) // Include Clients and their Locations
                                                 .SingleOrDefault(na => na.AccountId == accountId);

                        if (nationalAccount == null)
                        {
                            // If the national account does not exist, no need to delete
                            return false;
                        }

                        // Handle the Locations before deleting the Clients
                        foreach (var client in nationalAccount.Clients.ToList())
                        {
                            // Handle each location related to the client
                            foreach (var location in client.Locations.ToList())
                            {
                                // Delete the locations or handle them as necessary
                                ctx.Locations.Remove(location);
                            }

                            // After handling locations, now it's safe to delete the client
                            ctx.Clients.Remove(client);
                        }

                        // Save changes for the Locations and Clients
                        ctx.SaveChanges();

                        // Now it's safe to delete the NationalAccount
                        ctx.NationalAccounts.Remove(nationalAccount);
                        ctx.SaveChanges();

                        // Commit the transaction if all operations were successful
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction if any exception occurs
                        transaction.Rollback();
                        // Log the exception, rethrow, or handle as appropriate
                        throw;
                    }
                }
            }
        }

        public double GetTotalCO2SavedForFranchiseById(int accountId)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var account = ctx.NationalAccounts
                    .Include(f => f.Clients)
                    .SingleOrDefault(f => f.AccountId == accountId);

                if (account == null) return 0;

                double totalCO2Saved = 0;
                foreach (var client in account.Clients)
                {
                    totalCO2Saved += client.TotalCO2SavedV2;
                }

                return totalCO2Saved;
            }
        }

        public int CountDistinctStatesWithClientsByFranchiseId(int accountId)
        {
            using (var ctx = new ApplicationDbContext())
            {
                int stateReach = 0;
                var account = ctx.NationalAccounts
                                    .Include(f => f.Clients)
                                    .SingleOrDefault(f => f.AccountId == accountId);

                if (account != null)
                {
                    // Count distinct states of the clients within the franchise
                    return stateReach = account.Clients.Select(c => c.State).Distinct().Count();
                }

                return 0;
            }
        }

        public int CountClientsByFranchiseId(int accountId)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var account = ctx.NationalAccounts
                                    .Include(f => f.Clients)
                                    .SingleOrDefault(f => f.AccountId == accountId);

                if (account != null)
                {
                    // Count the clients within the franchise
                    return account.Clients.Count;
                }

                return 0;
            }
        }
    }
}
