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
        public string ChargeName { get; set; }
        public int WorkOrderId { get; set; }
        [ForeignKey("WorkOrderId")]
        public virtual WorkOrder WorkOrder { get; set; }
        public string Description { get; set; }
        public float Charge { get; set; }
        public float SubTotal { get; set; }
        public float Tax { get; set; }
        public float Total
        {
            get
            {
                var total = SubTotal + Tax;
                return total;
            }
        }
    }
}
