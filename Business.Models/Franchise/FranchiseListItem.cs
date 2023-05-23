using BusinessData.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessModels.Franchise
{
    public class FranchiseListItem
    {
        [Display(Name = "Franchise ID")]
        public int FranchiseId { get; set; }
        public bool IsActive { get; set; }


        [Display(Name = "Franchise")]
        public string FranchiseName { get; set; }
        public State State { get; set; }
        public string Status { get; set; }
        public string BusinessState { get; set; }

    }
}
