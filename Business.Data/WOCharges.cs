using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessData
{
    public class WOCharges
    {
        [Key]
        public int ChargeId { get; set; }
        public int VonigoChargeId { get; set; }
        public string ItemType { get; set; }
        public string VonigoInvoiceId { get; set; }
        public string ChargeName { get; set; }
        public string Title { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime EditedDate { get; set; }
        public bool ActiveCan { get; set; }
        public float SubTotal { get; set; }
        public float Tax { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public string Charge { get; set; }
        public int VonigoWorkOrderId { get; set; }
        public string VonigoWorkOrderName { get; set; }
        public string VonigoPriceListName { get; set; }
        public int VonigoFranchiseId { get; set; }
        public string FranchiseName { get; set; }
        public int FranchiseId { get; set; }
        [ForeignKey("FranchiseId")]
        public virtual Franchise Franchise { get; set; }
        public int WorkOrderId { get; set; }
        [ForeignKey("WorkOrderId")]
        public virtual WorkOrder WorkOrder { get; set; }
        public float Total { get; set; }
    }
}
