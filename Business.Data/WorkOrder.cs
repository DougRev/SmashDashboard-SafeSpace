using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessData
{
    public class WorkOrder
    {
        [Key]
        public int WorkOrderId { get; set; }
        public int VonigoWorkOrderId { get; set; }
        public int VonigoClientId { get; set; }
        public string Title { get; set; }
       
        public int FranchiseId { get; set; }
        [ForeignKey("FranchiseId")]
        public virtual Franchise Franchise { get; set; }
        public int InvoiceId { get; set; }
        [ForeignKey("InvoiceId")]
        public virtual Invoice Invoice { get; set; }

        public DateTime CompletedDate { get; set; }
        public DateTime CompletedTime { get; set; }
        public bool VonigoIsActive { get; set; }
        public int StreetNumber { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zipcode { get; set; }
        public int? ContactId { get; set; }
        [ForeignKey("ContactId")]
        public virtual Contact Contact { get; set; }
        public int DumpstersSmashed { get; set; }
        public string Summary { get; set; }
        public string OnSiteContact { get; set; }
        public string ServiceType { get; set; }
        public string VonigoLabel { get; set; }
        public int VonigoInvoiceId { get; set; }
        
        public int BeforeFullness { get; set; }
        public int AfterFullness { get; set; }
        public float TotalCharges { get; set; }
    }
}
