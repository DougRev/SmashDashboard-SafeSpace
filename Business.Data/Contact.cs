using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessData
{
    public class Contact
    {
        public int ContactId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName
        {
            get
            {
                string full = FirstName + " " + LastName;
                return full;
            }
        }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string AccountType { get; set; }

        // Client relationship
        [Required]
        public int ClientId { get; set; }
        [ForeignKey("ClientId")]
        public virtual Client Client { get; set; }

        // Location relationship
        public int LocationId { get; set; } // foreign key

        // added virtual modifier for lazy loading
        public virtual Location Location { get; set; }
    }
}
