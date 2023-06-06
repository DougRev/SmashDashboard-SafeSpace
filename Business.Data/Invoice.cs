using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BusinessData
{
    public class Invoice
    {
        [Key]
        public int InvoiceId { get; set; }

        public int FranchiseId { get; set; }
        [ForeignKey("FranchiseId")]
        public virtual Franchise Franchise { get; set; }

        public int ClientId { get; set; }
        [ForeignKey("ClientId")]
        public virtual Client Client { get; set; }

        public virtual ICollection<WorkOrder> WorkOrders { get; set; }
        public virtual ICollection<WOCharges> Charges { get; set; }
        public string AccountType { get; set; }
        public float TotalCost { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }

    }
}
