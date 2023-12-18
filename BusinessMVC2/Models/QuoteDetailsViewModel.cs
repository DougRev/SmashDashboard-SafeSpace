using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BusinessMVC2.Models
{
    public class QuoteDetailsViewModel
    {
        public string ClientName { get; set; }
        public string ClientEmail { get; set; }
        // Add more client-specific properties as needed

        public List<QuoteItem> QuoteItems { get; set; }
        public decimal TotalPrice { get; set; }

        public QuoteDetailsViewModel()
        {
            QuoteItems = new List<QuoteItem>();
        }
    }

    public class QuoteItem
    {
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}