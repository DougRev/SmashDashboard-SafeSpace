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
    public class AccountListItem
    {
        [Display(Name = "Account ID")]
        public int AccountId { get; set; }

        [Display(Name = "Account Name")]
        public string AccountName { get; set; }
        public State State { get; set; }
    }
}
