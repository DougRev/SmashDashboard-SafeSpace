using BusinessData;
using BusinessData.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BusinessModels.NationalAccount
{
    public class AccountDetails
    {
        [Display(Name = "Account ID")]
        public int AccountId { get; set; }

        [Display(Name = "Account Name")]
        public string AccountName { get; set; }
        public State State { get; set; }
        public List<Client> Clients { get; set; } = new List<Client>();
        public double TotalCO2Saved { get; set; }
        public int StateReach { get; set; }
        public int Locations { get; set; }
    }
}
