using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessShared
{
    public class QuoteResultViewModel
    {
        public string BusinessName { get; set; }
        public float PreSMTYearlyHauls { get; set; }
        public double TotalCO2BaselineTruckEmissionsV2 { get; set; }
        public double TotalCO2EmissionsWithSmashV2 { get; set; }
        public double TotalCO2SavedV2 { get; set; }
        public string CO2PercentSavedV2 { get; set; }
        public float LandfillDist { get; set; }
        public float HaulsWithSMT { get; set; }

    }
}
