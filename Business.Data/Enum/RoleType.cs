using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessData.Enum
{
    public enum RoleType
    {
        Owner,
        [Display(Name = "General Manager")]
        GeneralManager,
        Sales
    }
}
