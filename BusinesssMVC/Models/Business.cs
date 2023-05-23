using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BusinesssMVC.Models
{
    public class Business
    {
        [Key]
        public int BusinessId { get; set; }
        public string BusinessName { get; set; }
        public int? BusinessIdNumber { get; set; }

        public virtual ICollection<Franchisee> Franchisees { get; set; }
    }
}