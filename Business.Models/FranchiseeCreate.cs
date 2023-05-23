using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessModels
{
    public class FranchiseeCreate
    {
        public int FranchiseeId { get; set; }
        public string FranchiseeName { get; set; }
        public Guid OwnerId { get; set; }

    }
}
