using BusinessData.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessModels.NationalAccount
{
    public class AccountEdit
    {
        [Display(Name = "Account ID")]
        public int AccountId { get; set; }

        [Display(Name = "Account Name")]
        public string AccountName { get; set; }
        public State State { get; set; }
    }
}
