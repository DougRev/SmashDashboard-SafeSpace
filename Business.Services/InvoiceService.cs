using BusinessData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessServices
{
    public class InvoiceService
    {
        private ApplicationDbContext _context;

        public InvoiceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Invoice> GetInvoicesByVonigoClientId(int vonigoClientId)
        {
            return _context.Invoices
                           .Where(i => i.VonigoClientId == vonigoClientId)
                           .ToList();
        }
    }

}
