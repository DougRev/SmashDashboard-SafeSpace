using BusinessData;
using BusinessModels;
using BusinesssData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessServices
{
    public class FranchiseeService
    {
        private readonly Guid _userId;
        
        public FranchiseeService (Guid userId)
        {
            _userId = userId;
        }

        public bool CreateFranchisee(FranchiseeCreate model)
        {
            var entity = new FranchiseOwner()
            {
                FranchiseeId = model.FranchiseeId,
                FranchiseeName = model.FranchiseeName,
                OwnerId = _userId,
            };
            using (var ctx = new ApplicationDbContext())
            {
                ctx.Franchisees.Add(entity);
                return ctx.SaveChanges() == 1;
            }
        }

        public IEnumerable<FranchiseeListItem> GetFranchisees()
        {
            using (var ctx = new ApplicationDbContext())
            {
                var query = ctx.Franchisees
                    .Select(e => new FranchiseeListItem
                    {
                        FranchiseeId = e.FranchiseeId,
                        FranchiseeName = e.FranchiseeName,
                    });
                return query.ToArray();
            }
        }

        public FranchiseeDetails GetFranchiseeById(int franchiseeId)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity =
                    ctx
                    .Franchisees
                    .Single(e => e.FranchiseeId == franchiseeId);
                return
                new FranchiseeDetails
                {
                   FranchiseeId = entity.FranchiseeId,
                   FranchiseeName = entity.FranchiseeName,
                   
                };
            }
        }

        public bool UpdateFranchisee(FranchiseeEdit model)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity = ctx
                    .Franchisees
                    .Single(e => e.FranchiseeId == model.FranchiseeId );
                entity.FranchiseeName = model.FranchiseeName;

                return ctx.SaveChanges() == 1;
            }

        }
        public bool DeleteFranchisee(int franchiseeId)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity =
                    ctx
                    .Franchisees
                    .Single(e => e.FranchiseeId == franchiseeId );

                ctx.Franchisees.Remove(entity);

                return ctx.SaveChanges() == 1;
            }
        }
    }
}
