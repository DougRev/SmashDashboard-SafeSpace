using BusinessData.Interfaces;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BusinessServices
{
    public class UserIdProvider : IUserIdProvider
    {
        public Guid GetUserId()
        {
            return Guid.Parse(HttpContext.Current.User.Identity.GetUserId());
        }
    }

}
