using BusinessData;
using BusinessData.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BusinessModels.Franchise
{
    public class FranchiseDetails
    {
        [Display(Name = "Franchise ID")]
        public int FranchiseId { get; set; }

        [Display(Name = "Franchise Name")]
        public string FranchiseName { get; set; }

        [Display(Name = "Account ID")]
        public int? AccountId { get; set; }
        public State State { get; set; }
        public ICollection<Client> Clients { get; set; } = new List<Client>();

        [DisplayName("Is Active")]
        public bool IsActive { get; set; }

        [DisplayName("Region")]
        public string Region { get; set; }

        [DisplayName("Launch Date")]
        public DateTime? LaunchDate { get; set; }

        [DisplayName("Business Address 1")]
        public string BusinessAddress { get; set; }

        [DisplayName("Business Address City")]
        public string BusinessCity { get; set; }

        [DisplayName("Business Address Zip")]
        public string BusinessZipCode { get; set; }

        [DisplayName("Business Phone")]
        public string BusinessPhone { get; set; }

        [DisplayName("Status")]
        public string Status { get; set; }

        [DisplayName("Territory Info")]
        public string Territories { get; set; }

        [DisplayName("Owner 1 Name")]
        public string Owner1 { get; set; }

        [DisplayName("Owner 1 Email")]
        public string Owner1Email { get; set; }

        [DisplayName("Owner 1 Phone")]
        public string Owner1Phone { get; set; }

        [DisplayName("Owner 2 Name")]
        public string Owner2 { get; set; }

        [DisplayName("Owner 2 Email")]
        public string Owner2Email { get; set; }

        [DisplayName("Owner 2 Phone")]
        public string Owner2Phone { get; set; }

        [DisplayName("Owner 3 Name")]
        public string Owner3 { get; set; }

        [DisplayName("Owner 3 Email")]
        public string Owner3Email { get; set; }

        [DisplayName("Owner 3 Phone")]
        public string Owner3Phone { get; set; }

        [DisplayName("Owner 4 Name")]
        public string Owner4 { get; set; }

        [DisplayName("Owner 4 Email")]
        public string Owner4Email { get; set; }

        [DisplayName("Owner 4 Phone")]
        public string Owner4Phone { get; set; }

        public string BusinessState
        {
            get
            {
                return this.State.ToString();
            }
            set
            {
                State state;
                if (Enum.TryParse<State>(value, true, out state))
                {
                    this.State = state;
                }
                else
                {
                    this.State = default(State); // or however you wish to handle it
                }
            }
        }

    }
}
