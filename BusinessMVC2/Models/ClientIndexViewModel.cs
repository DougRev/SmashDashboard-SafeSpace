using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BusinessMVC2.Models
{
    public class ClientIndexViewModel
    {
        public IEnumerable<BusinessModels.BusinessListItem> Clients { get; set; }
        public IEnumerable<string> Franchises { get; set; }
        public IEnumerable<string> NationalAccounts { get; set; }
        public IEnumerable<string> States { get; set; }
    }

}