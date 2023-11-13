using BusinessData.Enum;
using BusinessData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessModels.NationalAccount
{
    public class AccountCreate
    {
        [Key]
        public int AccountId { get; set; }
        public Guid OwnerId { get; set; }
        [Display(Name = "Account Name")]
        public string AccountName { get; set; }
        public State State { get; set; }
        public List<Client> Clients { get; } = new List<Client>();
    }
}
