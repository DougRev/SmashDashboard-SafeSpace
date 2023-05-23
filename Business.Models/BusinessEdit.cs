using BusinessData.Enum;
using BusinesssData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml.Linq;

namespace BusinessModels
{
    public class BusinessEdit
    {
        [Display(Name = "Client ID")]
        public int BusinessId { get; set; }

        [Display(Name = "Client Name")]
        public string BusinessName { get; set; }

        [Display(Name = "Facility ID")]
        public string FacilityID { get; set; }
        public State State { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        [Display(Name = "Zip Code")]
        [Range(1, 99950, ErrorMessage = "Zip code must be between 00001 and 99950.")]
        public int ZipCode { get; set; }

        public Guid OwnerId { get; set; }

        [Display(Name = "Franchise")]
        public int FranchiseId { get; set; }
        public List<SelectListItem> Franchises { get; set; }

        [Display(Name = "National Account")]
        public int? AccountId { get; set; }
        public List<SelectListItem> NationalAccounts { get; set; }
        public string AccountName { get; set; }
        public string FranchiseName { get; set; }
        public int FranchiseeId { get; set; }
        public Compactibility Compactibility { get; set; }

        [Display(Name = "Transfer Station")]
        public bool XferStation { get; set; }

        [Display(Name = "SMT Distance to Client")]
        public float ToClientDist { get; set; }

        [Display(Name = "SMT Distance from Client")]
        public float FromClientDist { get; set; }

        [Display(Name = "Hauler Distance to Client")]
        public float ToHaulerDist { get; set; }

        [Display(Name = "Hauler Distance from Client")]
        public float FromHaulerDist { get; set; }

        [Display(Name = "Distance to Landfill (One Way)")]
        public float LandfillDist { get; set; }

        [Display(Name = "Hauls Per Week")]
        public int HaulsPerDay { get; set; }

        [Display(Name = "Number of Dumpsters")]
        public int NumberOfDumpsters { get; set; }

        [Display(Name = "Pre-SMT Est. Yearly Hauls")]
        public int PreSMTYearlyHauls { get; set; }

        //public virtual Franchisee Franchisee { get; set; }
    }
}
