using BusinessData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BusinesssData
{
    public class FranchiseOwner
    {
        [Key]
        public int FranchiseeId { get; set; }
        public Guid OwnerId { get; set; }
        public string FranchiseeName { get; set; }

        public int FranchiseId { get; set; }

        [ForeignKey ("FranchiseId")]
        public virtual Franchise Franchise { get; set; }

    }
}