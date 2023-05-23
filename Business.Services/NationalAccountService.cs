using BusinessData;
using BusinessModels.Franchise;
using BusinessModels.NationalAccount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;

namespace BusinessServices
{
    public class NationalAccountService
    {
        private readonly Guid _userId;

        public NationalAccountService(Guid userId)
        {
            _userId = userId;
        }
        public NationalAccountService() { }


        public bool CreateNationalAccount(AccountCreate model)
        {
            var entity = new NationalAccount()
            {
                OwnerId = _userId,
                AccountId = model.AccountId,
                AccountName = model.AccountName,
                State = model.State,
            };
            using (var ctx = new ApplicationDbContext())
            {
                ctx.NationalAccounts.Add(entity);
                return ctx.SaveChanges() == 1;
            }
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

        public bool DeleteNationalAccount(int accountId)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity = ctx.NationalAccounts.Single(e => e.AccountId == accountId);
                ctx.NationalAccounts.Remove(entity);
                return ctx.SaveChanges() == 1;
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
