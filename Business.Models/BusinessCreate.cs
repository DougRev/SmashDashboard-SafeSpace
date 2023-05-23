using BusinessData;
using BusinessData.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessModels
{
    public class BusinessCreate
    {
        public int? BusinessId { get; set; }

        [Display(Name = "Client Name")]
        public string BusinessName { get; set; }

        [Display(Name = "Franchise")]
        public int FranchiseId { get; set; }

        [Display(Name = "Franchise Name")]
        public string FranchiseName { get; set; }

        [Display(Name = "National Account")]
        public int? AccountId { get; set; }

        [Display(Name = "Account Name")]
        public string AccountName { get; set; }
        public Guid? OwnerId { get; set; }

        [Display(Name = "Facility ID")]
        public string FacilityID { get; set; }
        public State State { get; set; }
        public string City { get; set; }
        public string Address { get; set; }

        [Display(Name = "Zip Code")]
        [Range(1, 99950, ErrorMessage = "Zip code must be between 00001 and 99950.")]
        public int? ZipCode { get; set; }

        public Compactibility Compactibility { get; set; }
        public int FranchiseeId { get; set; }

        [Display(Name = "Number of Dumpsters")]
        public int NumberOfDumpsters { get; set; }

        [Display(Name = "Hauls Per Week")]
        public int HaulsPerDay { get; set; }

        [Display(Name = "Distance to Landfill (One Way)")]
        public float LandfillDist { get; set; }

        [Display(Name = "Save")]
        public bool AddToDb { get; set; }

        [Display(Name = "SMT Distance to Client")]
        public float ToClientDist { get; set; }

        [Display(Name = "SMT Distance from Client")]
        public float FromClientDist { get; set; }

        [Display(Name = "Hauler Distance to Client")]
        public float ToHaulerDist { get; set; }

        [Display(Name = "Hauler Distance to Next Customer")]
        public float FromHaulerDist { get; set; }

        [Display(Name = "Pre-SMT Est. Yearly Hauls ")]
        public int PreSMTYearlyHauls
        {
            get
            {
                int preSmtYearlyHauls = NumberOfDumpsters * HaulsPerDay * 52;
                return preSmtYearlyHauls;
            }
        }

        public double CompactibilityValue
        {
            get
            {
                double value = 0;
                return value = GetCompactibility(Compactibility);
            }
        }


        private double GetCompactibility(Compactibility compactibility)
        {
            switch (compactibility)
            {
                case Compactibility.High:
                    return .2;

                case Compactibility.Medium:
                    return .3;

                case Compactibility.Low:
                    return .4;
                default:
                    return 0;
            }
        }


        // CO2 Emissions

        public double CO2BaselineHaulerTruckRunningEmissionsV2
        {
            get
            {
                Emissions emissions = new Emissions();
                int yearlyHauls = PreSMTYearlyHauls;
                float vmt = (LandfillDist * 2);// + ToHaulerDist + FromHaulerDist;
                //float vmt = 34;
                double emissionFactor = emissions.RunningCO2;
                double conversionFactor = .002204622622;
                double baslineHaulerTruckRunningEmissions = yearlyHauls * vmt * emissionFactor * conversionFactor;
                return baslineHaulerTruckRunningEmissions;
            }
        }

        public double CO2BaselineHaulerTruckIdlingEmissions
        {
            get
            {
                Emissions emissions = new Emissions();
                int yearlyHauls = PreSMTYearlyHauls;
                double vit = 0.7;
                double emissionFactor = emissions.IdleCO2;
                double conversionFactor = .002204622622;
                double baselineHaulerTruckIdlingEmissions = yearlyHauls * vit * emissionFactor * conversionFactor;
                return baselineHaulerTruckIdlingEmissions;

            }
        }
        public double CO2SmashingEmissions
        {
            get
            {
                Emissions emissions = new Emissions();
                int yearlyHauls = PreSMTYearlyHauls;
                double smashingTime = .083333333;
                double emissionFactor = emissions.SmashCO2;
                double conversionFactor = .002204622622;
                double smashingEmissions = yearlyHauls * smashingTime * emissionFactor * conversionFactor;
                return smashingEmissions;

            }
        }

        public double CO2SMTRunningEmissions
        {
            get
            {
                Emissions emissions = new Emissions();
                double yearlyHauls = PreSMTYearlyHauls;
                //double roundTrip = ToClientDist + FromClientDist;
                double roundTrip = 2;
                double emissionFactor = emissions.SmashRunCO2;
                double conversionFactor = .002204622622;
                double runningEmissions = yearlyHauls * roundTrip * emissionFactor * conversionFactor;
                return runningEmissions;
            }
        }

        public double CO2SMTIdlingEmissions
        {
            get
            {
                Emissions emissions = new Emissions();
                double yearlyHauls = PreSMTYearlyHauls;
                double idlingTime = .083333333;
                double emissionFactor = emissions.SmashIdleCO2;
                double conversionFactor = .002204622622;
                double idlingEmissions = yearlyHauls * idlingTime * emissionFactor * conversionFactor;
                return idlingEmissions;
            }
        }

        public double CO2HaulerRunningEmissionsWithCompactibilityV2
        {
            get
            {
                double total = CO2BaselineHaulerTruckRunningEmissionsV2 * CompactibilityValue;
                return total;
            }
        }
        public double CO2HaulerIdlingEmissionsWithCompactibility
        {
            get
            {
                double total = CO2BaselineHaulerTruckIdlingEmissions * CompactibilityValue;
                return total;
            }
        }

        public double TotalCO2BaselineTruckEmissionsV2
        {
            get
            {

                double c02Total = CO2BaselineHaulerTruckRunningEmissionsV2 + CO2BaselineHaulerTruckIdlingEmissions;
                return c02Total;
            }
        }

        public double TotalCO2EmissionsWithSmashV2
        {
            get
            {
                double c02SmashTotal = CO2SmashingEmissions + CO2SMTRunningEmissions + CO2SMTIdlingEmissions + CO2HaulerRunningEmissionsWithCompactibilityV2 + CO2HaulerIdlingEmissionsWithCompactibility;
                return c02SmashTotal;
            }
        }

        public double TotalCO2SavedV2
        {
            get
            {
                double total = TotalCO2BaselineTruckEmissionsV2 - TotalCO2EmissionsWithSmashV2;
                return total;
            }
        }

        public string CO2PercentSavedV2
        {
            get
            {
                double saved = TotalCO2BaselineTruckEmissionsV2 - TotalCO2EmissionsWithSmashV2;
                double percent = saved / TotalCO2BaselineTruckEmissionsV2;
                string changed = string.Format("{0:P2}", percent);
                return changed;
            }
        }
    }
}
