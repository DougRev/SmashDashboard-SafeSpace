using BusinessData.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessData
{
    public class FranchiseRole
    {
        [Key] 
        public int FranchiseRoleId { get; set; } 
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public RoleType Role { get; set; }

        // Foreign key to associate the role with a specific franchise
        public int FranchiseId { get; set; }
    }
}
