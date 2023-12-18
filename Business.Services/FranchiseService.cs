using BusinessData;
using BusinessData.Enum;
using BusinessData.Interfaces;
using BusinessModels.Franchise;
using System;
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
                Roles = model.Roles != null ? model.Roles.Select(r => new FranchiseRole
                {
                    Name = r.Name,
                    Email = r.Email,
                    Phone = r.Phone,
                    Role = r.Role
                }).ToList() : new List<FranchiseRole>() // Initialize as an empty list if null
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
                var entity = ctx.Franchises
                                        .Include(f => f.Roles) // Ensure roles are loaded with the franchise
                                        .SingleOrDefault(e => e.FranchiseId == franchiseId);
                if (entity == null) return null;

                var franchiseDetails = new FranchiseDetails
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

                    // Assigning the Roles here
                    Roles = entity.Roles.Select(r => new BusinessModels.Franchise.FranchiseRoleModel
                    {
                        FranchiseId = r.FranchiseId,
                        FranchiseRoleId = r.FranchiseRoleId,
                        Name = r.Name,
                        Email = r.Email,
                        Phone = r.Phone,
                        Role = r.Role
                    }).ToList()
                };

                return franchiseDetails;
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

        public bool AddRoleToFranchise(int franchiseId, FranchiseRoleModel newRoleModel)
        {
            var franchise = _context.Franchises.Include(f => f.Roles).FirstOrDefault(f => f.FranchiseId == franchiseId);
            if (franchise == null)
            {
                return false;
            }

            if (newRoleModel.Email.EndsWith("@smashmytrash.com") && !ValidEmails.EmailList.Contains(newRoleModel.Email))
            {
                ValidEmails.EmailList.Add(newRoleModel.Email);
            }

            var newRole = new FranchiseRole
            {
                Name = newRoleModel.Name,
                Email = newRoleModel.Email,
                Phone = newRoleModel.Phone,
                Role = newRoleModel.Role, // Ensure this matches the enum in your data model
                FranchiseId = franchiseId  // Explicitly setting the FranchiseId
            };

            franchise.Roles.Add(newRole);

            // Check if the role is an owner role
            if (newRoleModel.Role == RoleType.Owner) // Adjust the condition based on how you define an owner
            {
                // Check and assign to the first empty owner field
                if (string.IsNullOrEmpty(franchise.Owner1) && string.IsNullOrEmpty(franchise.Owner1Email))
                {
                    franchise.Owner1 = newRoleModel.Name;
                    franchise.Owner1Email = newRoleModel.Email;
                }
                else if (string.IsNullOrEmpty(franchise.Owner2) && string.IsNullOrEmpty(franchise.Owner2Email))
                {
                    franchise.Owner2 = newRoleModel.Name;
                    franchise.Owner2Email = newRoleModel.Email;
                }
                else if (string.IsNullOrEmpty(franchise.Owner3) && string.IsNullOrEmpty(franchise.Owner3Email))
                {
                    franchise.Owner3 = newRoleModel.Name;
                    franchise.Owner3Email = newRoleModel.Email;
                }
                else if (string.IsNullOrEmpty(franchise.Owner4) && string.IsNullOrEmpty(franchise.Owner4Email))
                {
                    franchise.Owner4 = newRoleModel.Name;
                    franchise.Owner4Email = newRoleModel.Email;
                }
            }

            return _context.SaveChanges() > 0;
        }


        public FranchiseRoleModel GetRoleById(int roleId)
        {
            var roleEntity = _context.FranchiseRoles.Find(roleId);
            if (roleEntity == null)
            {
                return null;
            }

            return new FranchiseRoleModel
            {
                FranchiseRoleId = roleEntity.FranchiseRoleId,
                Name = roleEntity.Name,
                Email = roleEntity.Email,
                Phone = roleEntity.Phone,
                Role = roleEntity.Role,
            };
        }


        public bool UpdateRole(int roleId, FranchiseRoleModel updatedRoleModel)
        {
            var role = _context.FranchiseRoles.Find(roleId);
            if (role == null)
            {
                return false;
            }

            // Update role properties
            role.Name = updatedRoleModel.Name;
            role.Email = updatedRoleModel.Email;
            role.Phone = updatedRoleModel.Phone;
            role.Role = updatedRoleModel.Role;

            // Check if the role is an owner role
            if (updatedRoleModel.Role == RoleType.Owner) // Adjust the condition based on how you define an owner
            {
                // Find the associated franchise
                var franchise = _context.Franchises.FirstOrDefault(f => f.FranchiseId == role.FranchiseId); // Assuming there's a FranchiseId in the role

                // Check and assign to the first empty owner field
                if (string.IsNullOrEmpty(franchise.Owner1) && string.IsNullOrEmpty(franchise.Owner1Email))
                {
                    franchise.Owner1 = updatedRoleModel.Name;
                    franchise.Owner1Email = updatedRoleModel.Email;
                }
                else if (string.IsNullOrEmpty(franchise.Owner2) && string.IsNullOrEmpty(franchise.Owner2Email))
                {
                    franchise.Owner2 = updatedRoleModel.Name;
                    franchise.Owner2Email = updatedRoleModel.Email;
                }
                else if (string.IsNullOrEmpty(franchise.Owner3) && string.IsNullOrEmpty(franchise.Owner3Email))
                {
                    franchise.Owner3 = updatedRoleModel.Name;
                    franchise.Owner3Email = updatedRoleModel.Email;
                }
                else if (string.IsNullOrEmpty(franchise.Owner4) && string.IsNullOrEmpty(franchise.Owner4Email))
                {
                    franchise.Owner4 = updatedRoleModel.Name;
                    franchise.Owner4Email = updatedRoleModel.Email;
                }
            }

                // Save all changes
                return _context.SaveChanges() > 0;
        }


        public bool DeleteRole(int roleId)
        {
            var role = _context.FranchiseRoles.Find(roleId);
            if (role == null)
            {
                return false;
            }

            // Check if the role is an owner role and remove from franchise
            if (role.Role == RoleType.Owner)
            {
                var franchise = _context.Franchises.FirstOrDefault(f => f.FranchiseId == role.FranchiseId);
                if (franchise != null)
                {
                    if (franchise.Owner1 == role.Name) 
                    {
                        franchise.Owner1 = null;
                        franchise.Owner1Email = null;
                    }
                    else if (franchise.Owner2 == role.Name)
                    {
                        franchise.Owner2 = null;
                        franchise.Owner2Email = null;
                    }
                    else if (franchise.Owner3 == role.Name)
                    {
                        franchise.Owner3 = null;
                        franchise.Owner3Email = null;
                    }
                    else if (franchise.Owner4 == role.Name)
                    {
                        franchise.Owner4 = null;
                        franchise.Owner4Email = null;
                    }
                }
            }

            _context.FranchiseRoles.Remove(role);
            return _context.SaveChanges() > 0;
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

        public IEnumerable<FranchiseDetails> GetFranchisesWithOutOfStateClients(int threshold)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var franchises = ctx.Franchises.Include(f => f.Clients).ToList();
                var result = new List<FranchiseDetails>();

                foreach (var franchise in franchises)
                {
                    var franchiseState = ConvertToEnum(franchise.BusinessState);
                    if (!franchiseState.HasValue)
                    {
                        continue; // Skip if the franchise state is not valid
                    }

                    // Count clients whose State does not match franchiseState
                    int outOfStateCount = franchise.Clients
                        .Count(c => c.State != franchiseState.Value);

                    if (outOfStateCount > threshold)
                    {
                        result.Add(new FranchiseDetails
                        {
                            // ... other properties
                            FranchiseId = franchise.FranchiseId,
                            FranchiseName = franchise.FranchiseName,
                            State = franchiseState.Value,
                            OutOfStateClientCount = outOfStateCount,
                        });
                    }
                }

                return result;
            }
        }
        public List<RoleType> GetRolesForUser(string userEmail)
        {
            var userRoles = _context.FranchiseRoles
                                    .Where(role => role.Email.Equals(userEmail, StringComparison.OrdinalIgnoreCase))
                                    .Select(role => role.Role)
                                    .Distinct()
                                    .ToList();

            return userRoles;
        }


        // Add a method to get franchises for a specific set of roles
        public IEnumerable<FranchiseListItem> GetFranchisesForRoles(IEnumerable<RoleType> roles)
        {
            return _context.Franchises
                .Where(f => f.Roles.Any(role => roles.Contains(role.Role)))
                .Select(f => new FranchiseListItem
                {
                    // Map properties
                })
                .ToList();
        }


        public State? ConvertToEnum(string stateAbbreviation)
        {
            if (Enum.TryParse<State>(stateAbbreviation, out var stateEnum))
            {
                return stateEnum;
            }
            return null;
        }


        public bool ClearOwner4(int franchiseId)
        {
            franchiseId = 152;
            var franchise = _context.Franchises.Find(franchiseId);
            if (franchise == null)
            {
                return false;
            }

            // Clearing the Owner4 field
            franchise.Owner4 = null;
            franchise.Owner4Email = null; // If you have an Owner4Email field, clear it as well

            // Save the changes
            return _context.SaveChanges() > 0;
        }

    }
}
