using BusinessData.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BusinessData
{
    public class TestClient
    {

        [Key]
        public int BusinessId { get; set; }
        public string BusinessName { get; set; }
        public int? StreetNumber { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public State State { get; set; }

        [Display(Name = "Zip Code")]
        public string ZipCode { get; set; }
        public string ServiceLocation { get; set; }
        public Guid OwnerId { get; set; }
        public int VonigoFranchiseId { get; set; }
        public int VonigoClientId { get; set; }
        public int FranchiseId { get; set; }
        public int? AccountId { get; set; }
        public string AccountName { get; set; }
        public string FranchiseName { get; set; }

        [Display(Name = "Contact First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Contact Last Name")]
        public string LastName { get; set; }
        public string ContactName { get; set; }

        [Display(Name = "Phone Number")]
        public int PhoneNumber { get; set; }

        [Display(Name = "Email Address")]
        public string Email { get; set; }
    }
}
