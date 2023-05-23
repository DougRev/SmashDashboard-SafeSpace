using BusinessData.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessData
{
    public class NationalAccount
    {
        [Key]
        public int AccountId { get; set; }
        public Guid OwnerId { get; set; }
        public string AccountName { get; set; }
        public virtual State State { get; set; }
        public ICollection<Client> Clients { get; set; } = new List<Client>();

        public List<Client> GetClientsByFranchiseId(int franchiseId)
        {
            return Clients.Where(c => c.FranchiseId == franchiseId).ToList();
        }
    }
}
