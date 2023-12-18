using BusinessData;
using BusinessData.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml.Linq;

namespace BusinessModels.Franchise
{
    public class FranchiseEdit
    {
        [Display(Name = "Franchise ID")]
        public int FranchiseId { get; set; }
        [Display(Name = "Franchise Name")]
        public string FranchiseName { get; set; }
        [Required]
        public State State { get; set; }

        [Display(Name = "National Account")]
        public int? AccountId { get; set; }
        public List<SelectListItem> NationalAccounts { get; set; }
        public bool IsActive { get; set; }

        [DisplayName("Owner 1 Name")]
        public string Owner1 { get; set; }
        public string Owner1Email { get; set; }


        [DisplayName("Owner 2 Name")]
        public string Owner2 { get; set; }
        public string Owner2Email { get; set; }


        [DisplayName("Owner 3 Name")]
        public string Owner3 { get; set; }
        public string Owner3Email { get; set; }


        [DisplayName("Owner 4 Name")]
        public string Owner4 { get; set; }
        public string Owner4Email { get; set; }

        //public Guid OwnerId { get; set; }
        public List<FranchiseRoleModel> Roles { get; set; }
        public FranchiseRoleModel NewRole { get; set; }

    }
}
