using BusinessData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BusinessMVC2.Models
{
    public class ClientFormViewModel
    {
        public Client Client { get; set; }
        public Franchise Franchise { get; set; }
        // Add more properties as needed
    }
}