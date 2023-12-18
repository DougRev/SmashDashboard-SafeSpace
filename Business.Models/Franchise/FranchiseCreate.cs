using BusinessData;
using BusinessData.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessModels.Franchise
{
    public class FranchiseCreate
    {
        [Key]
        [Display(Name = "Franchise ID")]
        public int FranchiseId { get; set; }

        [Display(Name = "Account Name")]
        public string FranchiseName { get; set; }

        [DisplayName("Owner 1 Name")]
        public string Owner1 { get; set; }

        [DisplayName("Owner 2 Name")]
        public string Owner2 { get; set; }

        [DisplayName("Owner 3 Name")]
        public string Owner3 { get; set; }

        [DisplayName("Owner 4 Name")]
        public string Owner4 { get; set; }
        public Guid OwnerId { get; set; }
        [Required]
        public State State { get; set; }
        public List<Client> Clients { get; set; }
        public List<FranchiseRoleModel> Roles { get; set; }

        public class FranchiseRoleModel
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public RoleType Role { get; set; } // Assuming RoleType is an enum defined elsewhere
        }

    }
}
