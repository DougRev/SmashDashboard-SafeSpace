using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessData
{
    public class Location
    {
        public int LocationId { get; set; }
        //public string StreetNumber { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Providence { get; set; }
        public string ZipCode { get; set; }
        public string AccountType { get; set; }
        [Required]
        public int ClientId { get; set; }
        [ForeignKey("ClientId")]
        public virtual Client Client { get; set; }
        public int? ContactId { get; set; }
        [ForeignKey("ContactId")]
        public virtual Contact Contact { get; set; }
        // added virtual modifier for lazy loading
        public virtual ICollection<Contact> Contacts { get; set; }

        public Location()
        {
            Contacts = new HashSet<Contact>();
        }
    }
}
