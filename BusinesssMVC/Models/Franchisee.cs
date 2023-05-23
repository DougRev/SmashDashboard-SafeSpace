using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BusinesssMVC.Models
{
    public class Franchisee
    {
        [Key]
        public int FranchiseeId { get; set; }
        public string FranchiseeName { get; set; }
    }
}