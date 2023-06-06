using BusinessData.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessData
{
    public class Franchise
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FranchiseId { get; set; }
        public bool IsActive { get; set; }
        public Guid OwnerId { get; set; }

        [DisplayName("Franchise Name")]
        public string FranchiseName { get; set; }

        [Display(Name = "Account ID")]
        public int? AccountId { get; set; }

        [DisplayName("Client State")]
        public virtual State State { get; set; }

        //public List<Client> Clients { get; } = new List<Client>();
        public ICollection<Client> Clients { get; set; } = new List<Client>();
        public virtual ICollection<Invoice> Invoices { get; set; }


        public List<Client> GetClientsByFranchiseId(int franchiseId)
        {
            return Clients.Where(c => c.FranchiseId == franchiseId).ToList();
        }


        [DisplayName("Region")]
        public string Region { get; set; }

        [DisplayName("Launch Date")]
        public DateTime? LaunchDate { get; set; }

        [DisplayName("Business Address 1")]
        public string BusinessAddress { get; set; }

        [DisplayName("Business Address City")]
        public string BusinessCity { get; set; }

        [DisplayName("State")]
        public string BusinessState { get; set; }

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

        [DisplayName("Owner 3Email")]
        public string Owner3Email { get; set; }

        [DisplayName("Owner 3 Phone")]
        public string Owner3Phone { get; set; }

        [DisplayName("Owner 1 Name")]
        public string Owner4 { get; set; }

        [DisplayName("Owner 4 Email")]
        public string Owner4Email { get; set; }

        [DisplayName("Owner 4 Phone")]
        public string Owner4Phone { get; set; }

    }
}
