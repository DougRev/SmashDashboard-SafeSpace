using BusinessData.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessModels.Franchise
{
    public class FranchiseRoleModel
    {
        public int FranchiseId { get; set; }
        public int FranchiseRoleId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public RoleType Role { get; set; }
    }
}
