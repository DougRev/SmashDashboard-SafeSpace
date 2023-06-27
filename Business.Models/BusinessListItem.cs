using BusinessData.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessModels
{
    public class BusinessListItem
    {
        [Display(Name = "Client ID")]
        public int BusinessId { get; set; }

        [Display(Name = "Client Name")]
        public string BusinessName { get; set; }
        [Display(Name = "Franchise Id")]
        public int FranchiseId { get; set; }
        [Display(Name = "Franchise")]
        public string FranchiseName { get; set; }

        [Display(Name = "National Account")]
        public string AccountName { get; set; }

        [Display(Name = "Facility ID")]
        public string FacilityID { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public State State { get; set; }

        [Display(Name = "Zip Code")]
        public string ZipCode { get; set; }
        public Guid OwnerId { get; set; }
        [DisplayName("Service Location")]
        public string ServiceLocation { get; set; }
        public int NumberOfDumpsters { get; set; }

    }
}
