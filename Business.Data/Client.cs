using BusinessData.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessData
{
    public class Client
    {
        [Key]
        public int BusinessId { get; set; }
        public bool IsActive { get; set; }
        public string BusinessName { get; set; }

        [Display(Name = "Facility ID")]
        public string FacilityID { get; set; }
        public int? StreetNumber { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public State State { get; set; }

        [Display(Name = "Zip Code")]
        public string ZipCode { get; set; }
        public string ServiceLocation { get; set; }
        public Guid OwnerId { get; set; }
        public int VonigoFranchiseId { get; set; }
        public int VonigoClientId { get; set; }

        public int FranchiseId { get; set; }

        [ForeignKey("FranchiseId")]
        public virtual Franchise Franchise { get; set; }
        public int? AccountId { get; set; }

        [ForeignKey("AccountId")]
        public virtual NationalAccount NationalAccount { get; set; }
        public virtual ICollection<Location> Locations { get; set; }
        public virtual ICollection<Contact> Contacts { get; set; }
        public virtual ICollection<Invoice> Invoices { get; set; }

        public string AccountName { get; set; }
        public string FranchiseName { get; set; }

        [Display(Name = "Contact First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Contact Last Name")]
        public string LastName { get; set; }
        public string ContactName { get; set; }

        [Display(Name = "Phone Number")]
        public int PhoneNumber { get; set; }

        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Display(Name = "Save")]
        public bool AddToDb { get; set; }

        [Display(Name = "SMT Distance to Client")]
        public float ToClientDist { get; set; }

        [Display(Name = "SMT Distance from Client")]
        public float FromClientDist { get; set; }

        [Display(Name = "Hauler Distance to Client")]
        public float ToHaulerDist { get; set; }

        [Display(Name = "Hauler Distance to Landfill")]
        public float LandfillDist { get; set; }

        [Display(Name = "Hauler Distance to Next Customer")]
        public float FromHaulerDist { get; set; }

        [Display(Name = "Hauls Per Week")]
        public int HaulsPerDay { get; set; }

        [Display(Name = "Number of Dumpsters")]
        public int NumberOfDumpsters { get; set; }

        [Display(Name = "Pre-SMT Est. Yearly Hauls ")]
        public int PreSMTYearlyHauls
        {
            get
            {
                int preSmtYearlyHauls =  NumberOfDumpsters * HaulsPerDay * 52; 
                return preSmtYearlyHauls;
            }
        }

        public Compactibility Compactibility { get; set; }
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




        //Emissions Added Totals


        public double AllEmissionsBaselineTotalsV2
        {
            get
            {
                double total = TotalNOXBaselineTruckEmissionsV2 + TotalN20BaselineTruckEmissionsV2 + TotalPM25BaselineTruckEmissionsV2 + TotalPM10BaselineTruckEmissionsV2 + TotalSO2BaselineTruckEmissionsV2 + TotalCH4BaselineTruckEmissionsV2 + TotalCOBaselineTruckEmissionsV3 + TotalVOCBaselineTruckEmissionsV2 + TotalCO2BaselineTruckEmissionsV2;
                return total;
            }
        }

        public double AllEmissionsWithSmashTotalsV2
        {
            get
            {
                double total = TotalNOXEmissionsWithSmashV2 + TotalN20EmissionsWithSmashV2 + TotalPM25EmissionsWithSmashV2 + TotalPM10EmissionsWithSmashV2 + TotalSO2EmissionsWithSmashV2 + TotalCH4EmissionsWithSmashV2 + TotalCOEmissionsWithSmashV3 + TotalVOCEmissionsWithSmashV2 + TotalCO2EmissionsWithSmashV2;
                return total;
            }
        }

        public double AllEmissionsSavedWithSmashV2
        {
            get
            {
                double saved = AllEmissionsBaselineTotalsV2 - AllEmissionsWithSmashTotalsV2;
                return saved;
            }
        }

        public string AllSavingsTotalV2
        {
            get
            {
                double percent = AllEmissionsSavedWithSmashV2 / AllEmissionsBaselineTotalsV2;
                string changed = string.Format("{0:P2}", percent);
                return changed;
            }
        }


        // NOx Emissions

        public double NOXBaselineHaulerTruckRunningEmissionsV2
        {
            get
            {
                Emissions emissions = new Emissions();
                int yearlyHauls = PreSMTYearlyHauls;
                float vmt = (LandfillDist * 2);// + ToHaulerDist + FromHaulerDist;
                //float vmt = 34;
                double emissionFactor = emissions.RunningNOX;
                double conversionFactor = .002204622622;
                double baslineHaulerTruckRunningEmissions = yearlyHauls * vmt * emissionFactor * conversionFactor;
                return baslineHaulerTruckRunningEmissions;
            }
        }
        public double NOXBaselineHaulerTruckIdlingEmissions
        {
            get
            {
                Emissions emissions = new Emissions();
                int yearlyHauls = PreSMTYearlyHauls;
                double vit = 0.722916667;
                double emissionFactor = emissions.IdleNOX;
                double conversionFactor = .002204622622;
                double baselineHaulerTruckIdlingEmissions = yearlyHauls * vit * emissionFactor * conversionFactor;
                return baselineHaulerTruckIdlingEmissions;

            }
        }

        public double NOXSmashingEmissions
        {
            get
            {
                Emissions emissions = new Emissions();
                int yearlyHauls = PreSMTYearlyHauls;
                double smashingTime = .083333333;
                double emissionFactor = emissions.SmashNOX;
                double conversionFactor = .002204622622;
                double smashingEmissions = yearlyHauls * smashingTime * emissionFactor * conversionFactor;
                return smashingEmissions;

            }
        }

        public double NOXSMTRunningEmissions
        {
            get
            {
                Emissions emissions = new Emissions();
                double yearlyHauls = PreSMTYearlyHauls;
                //double roundTrip = ToClientDist + FromClientDist;
                double roundTrip = 2;
                double emissionFactor = emissions.SmashRunNOX;
                double conversionFactor = .002204622622;
                double runningEmissions = yearlyHauls * roundTrip * emissionFactor * conversionFactor;
                return runningEmissions;
            }
        }

        public double NOXSMTIdlingEmissions
        {
            get
            {
                Emissions emissions = new Emissions();
                double yearlyHauls = PreSMTYearlyHauls;
                double idlingTime = .083333333;
                double emissionFactor = emissions.SmashIdleNOX;
                double conversionFactor = .002204622622;
                double idlingEmissions = yearlyHauls * idlingTime * emissionFactor * conversionFactor;
                return idlingEmissions;
            }
        }

        public double NOXHaulerRunningEmissionsWithCompactibilityV2
        {
            get
            {
                double total = NOXBaselineHaulerTruckRunningEmissionsV2 * CompactibilityValue;
                return total;
            }
        }

        public double NOXHaulerIdlingEmissionsWithCompactibility
        {
            get
            {
                double total = NOXBaselineHaulerTruckIdlingEmissions * CompactibilityValue;
                return total;
            }
        }
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double TotalNOXBaselineTruckEmissionsV2
        {
            get
            {

                double c02Total = NOXBaselineHaulerTruckRunningEmissionsV2 + NOXBaselineHaulerTruckIdlingEmissions;
                return c02Total;
            }
        }
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double TotalNOXEmissionsWithSmashV2
        {
            get
            {
                double c02SmashTotal = NOXSmashingEmissions + NOXSMTRunningEmissions + NOXSMTIdlingEmissions + NOXHaulerRunningEmissionsWithCompactibilityV2 + NOXHaulerIdlingEmissionsWithCompactibility;
                return c02SmashTotal;
            }
        }

        public string NOXPercentSavedV2
        {
            get
            {
                double saved = TotalNOXBaselineTruckEmissionsV2 - TotalNOXEmissionsWithSmashV2;
                double percent = saved / TotalNOXBaselineTruckEmissionsV2;
                string changed = string.Format("{0:P2}", percent);
                return changed;
            }
        }

        // N20 Emissions

        public double N20BaselineHaulerTruckRunningEmissionsV2
        {
            get
            {
                Emissions emissions = new Emissions();
                int yearlyHauls = PreSMTYearlyHauls;
                float vmt = (LandfillDist * 2);// + ToHaulerDist + FromHaulerDist;
                //float vmt = 34;
                double emissionFactor = emissions.RunningN20;
                double conversionFactor = .002204622622;
                double baslineHaulerTruckRunningEmissions = yearlyHauls * vmt * emissionFactor * conversionFactor;
                return baslineHaulerTruckRunningEmissions;
            }
        }

        public double N20BaselineHaulerTruckIdlingEmissions
        {
            get
            {
                Emissions emissions = new Emissions();
                int yearlyHauls = PreSMTYearlyHauls;
                double vit = 0.7;
                double emissionFactor = emissions.IdleN20;
                double conversionFactor = .002204622622;
                double baselineHaulerTruckIdlingEmissions = yearlyHauls * vit * emissionFactor * conversionFactor;
                return baselineHaulerTruckIdlingEmissions;

            }
        }
        public double N20SmashingEmissions
        {
            get
            {
                Emissions emissions = new Emissions();
                int yearlyHauls = PreSMTYearlyHauls;
                double smashingTime = .083333333;
                double emissionFactor = emissions.SmashN20;
                double conversionFactor = .002204622622;
                double smashingEmissions = yearlyHauls * smashingTime * emissionFactor * conversionFactor;
                return smashingEmissions;

            }
        }
        public double N20SMTRunningEmissions
        {
            get
            {
                Emissions emissions = new Emissions();
                double yearlyHauls = PreSMTYearlyHauls;
                //double roundTrip = ToClientDist + FromClientDist;
                double roundTrip = 2;
                double emissionFactor = emissions.SmashRunN20;
                double conversionFactor = .002204622622;
                double runningEmissions = yearlyHauls * roundTrip * emissionFactor * conversionFactor;
                return runningEmissions;
            }
        }
        public double N20SMTIdlingEmissions
        {
            get
            {
                Emissions emissions = new Emissions();
                double yearlyHauls = PreSMTYearlyHauls;
                double idlingTime = .083333333;
                double emissionFactor = emissions.SmashIdleN20;
                double conversionFactor = .002204622622;
                double idlingEmissions = yearlyHauls * idlingTime * emissionFactor * conversionFactor;
                return idlingEmissions;
            }
        }

        public double N20HaulerRunningEmissionsWithCompactibilityV2
        {
            get
            {
                double total = N20BaselineHaulerTruckRunningEmissionsV2 * CompactibilityValue;
                return total;
            }
        }
        public double N20HaulerIdlingEmissionsWithCompactibility
        {
            get
            {
                double total = N20BaselineHaulerTruckIdlingEmissions * CompactibilityValue;
                return total;
            }
        }

        public double TotalN20BaselineTruckEmissionsV2
        {
            get
            {

                double c02Total = N20BaselineHaulerTruckRunningEmissionsV2 + N20BaselineHaulerTruckIdlingEmissions;
                return c02Total;
            }
        }

        public double TotalN20EmissionsWithSmashV2
        {
            get
            {
                double c02SmashTotal = N20SmashingEmissions + N20SMTRunningEmissions + N20SMTIdlingEmissions + N20HaulerRunningEmissionsWithCompactibilityV2 + N20HaulerIdlingEmissionsWithCompactibility;
                return c02SmashTotal;
            }
        }

        public string N20PercentSavedV2
        {
            get
            {
                double saved = TotalN20BaselineTruckEmissionsV2 - TotalN20EmissionsWithSmashV2;
                double percent = saved / TotalN20BaselineTruckEmissionsV2;
                string changed = string.Format("{0:P2}", percent);
                return changed;
            }
        }

        // PM2.5 Emissions

        public double PM25BaselineHaulerTruckRunningEmissionsV2
        {
            get
            {
                Emissions emissions = new Emissions();
                int yearlyHauls = PreSMTYearlyHauls;
                float vmt = (LandfillDist * 2);// + ToHaulerDist + FromHaulerDist;
                //float vmt = 34;
                double emissionFactor = emissions.RunningPM25;
                double conversionFactor = .002204622622;
                double baslineHaulerTruckRunningEmissions = yearlyHauls * vmt * emissionFactor * conversionFactor;
                return baslineHaulerTruckRunningEmissions;
            }
        }

        public double PM25BaselineHaulerTruckIdlingEmissions
        {
            get
            {
                Emissions emissions = new Emissions();
                int yearlyHauls = PreSMTYearlyHauls;
                double vit = 0.7;
                double emissionFactor = emissions.IdlePM25;
                double conversionFactor = .002204622622;
                double baselineHaulerTruckIdlingEmissions = yearlyHauls * vit * emissionFactor * conversionFactor;
                return baselineHaulerTruckIdlingEmissions;

            }
        }
        public double PM25SmashingEmissions
        {
            get
            {
                Emissions emissions = new Emissions();
                int yearlyHauls = PreSMTYearlyHauls;
                double smashingTime = .083333333;
                double emissionFactor = emissions.SmashPM25;
                double conversionFactor = .002204622622;
                double smashingEmissions = yearlyHauls * smashingTime * emissionFactor * conversionFactor;
                return smashingEmissions;

            }
        }
        public double PM25SMTRunningEmissions
        {
            get
            {
                Emissions emissions = new Emissions();
                double yearlyHauls = PreSMTYearlyHauls;
                //double roundTrip = ToClientDist + FromClientDist;
                double roundTrip = 2;
                double emissionFactor = emissions.SmashRunPM25;
                double conversionFactor = .002204622622;
                double runningEmissions = yearlyHauls * roundTrip * emissionFactor * conversionFactor;
                return runningEmissions;
            }
        }
        public double PM25SMTIdlingEmissions
        {
            get
            {
                Emissions emissions = new Emissions();
                double yearlyHauls = PreSMTYearlyHauls;
                double idlingTime = .083333333;
                double emissionFactor = emissions.SmashIdlePM25;
                double conversionFactor = .002204622622;
                double idlingEmissions = yearlyHauls * idlingTime * emissionFactor * conversionFactor;
                return idlingEmissions;
            }
        }

        public double PM25HaulerRunningEmissionsWithCompactibilityV2
        {
            get
            {
                double total = PM25BaselineHaulerTruckRunningEmissionsV2 * CompactibilityValue;
                return total;
            }
        }
        public double PM25HaulerIdlingEmissionsWithCompactibility
        {
            get
            {
                double total = PM25BaselineHaulerTruckIdlingEmissions * CompactibilityValue;
                return total;
            }
        }

        public double TotalPM25BaselineTruckEmissionsV2
        {
            get
            {

                double c02Total = PM25BaselineHaulerTruckRunningEmissionsV2 + PM25BaselineHaulerTruckIdlingEmissions;
                return c02Total;
            }
        }
        public double TotalPM25EmissionsWithSmashV2
        {
            get
            {
                double c02SmashTotal = PM25SmashingEmissions + PM25SMTRunningEmissions + PM25SMTIdlingEmissions + PM25HaulerRunningEmissionsWithCompactibilityV2 + PM25HaulerIdlingEmissionsWithCompactibility;
                return c02SmashTotal;
            }
        }

        public string PM25PercentSavedV2
        {
            get
            {
                double saved = TotalPM25BaselineTruckEmissionsV2 - TotalPM25EmissionsWithSmashV2;
                double percent = saved / TotalPM25BaselineTruckEmissionsV2;
                string changed = string.Format("{0:P2}", percent);
                return changed;
            }
        }

        // PM10 Emissions

        public double PM10BaselineHaulerTruckRunningEmissionsV2
        {
            get
            {
                Emissions emissions = new Emissions();
                int yearlyHauls = PreSMTYearlyHauls;
                float vmt = (LandfillDist * 2);// + ToHaulerDist + FromHaulerDist;
                //float vmt = 34;
                double emissionFactor = emissions.RunningPM10;
                double conversionFactor = .002204622622;
                double baslineHaulerTruckRunningEmissions = yearlyHauls * vmt * emissionFactor * conversionFactor;
                return baslineHaulerTruckRunningEmissions;
            }
        }

        public double PM10BaselineHaulerTruckIdlingEmissions
        {
            get
            {
                Emissions emissions = new Emissions();
                int yearlyHauls = PreSMTYearlyHauls;
                double vit = 0.7;
                double emissionFactor = emissions.IdlePM10;
                double conversionFactor = .002204622622;
                double baselineHaulerTruckIdlingEmissions = yearlyHauls * vit * emissionFactor * conversionFactor;
                return baselineHaulerTruckIdlingEmissions;

            }
        }
        public double PM10SmashingEmissions
        {
            get
            {
                Emissions emissions = new Emissions();
                int yearlyHauls = PreSMTYearlyHauls;
                double smashingTime = .083333333;
                double emissionFactor = emissions.SmashPM10;
                double conversionFactor = .002204622622;
                double smashingEmissions = yearlyHauls * smashingTime * emissionFactor * conversionFactor;
                return smashingEmissions;

            }
        }
        public double PM10SMTRunningEmissions
        {
            get
            {
                Emissions emissions = new Emissions();
                double yearlyHauls = PreSMTYearlyHauls;
                //double roundTrip = ToClientDist + FromClientDist;
                double roundTrip = 2;
                double emissionFactor = emissions.SmashRunPM10;
                double conversionFactor = .002204622622;
                double runningEmissions = yearlyHauls * roundTrip * emissionFactor * conversionFactor;
                return runningEmissions;
            }
        }

        public double PM10SMTIdlingEmissions
        {
            get
            {
                Emissions emissions = new Emissions();
                double yearlyHauls = PreSMTYearlyHauls;
                double idlingTime = .083333333;
                double emissionFactor = emissions.SmashIdlePM10;
                double conversionFactor = .002204622622;
                double idlingEmissions = yearlyHauls * idlingTime * emissionFactor * conversionFactor;
                return idlingEmissions;
            }
        }

        public double PM10HaulerRunningEmissionsWithCompactibilityV2
        {
            get
            {
                double total = PM10BaselineHaulerTruckRunningEmissionsV2 * CompactibilityValue;
                return total;
            }
        }
        public double PM10HaulerIdlingEmissionsWithCompactibility
        {
            get
            {
                double total = PM10BaselineHaulerTruckIdlingEmissions * CompactibilityValue;
                return total;
            }
        }

        public double TotalPM10BaselineTruckEmissionsV2
        {
            get
            {

                double c02Total = PM10BaselineHaulerTruckRunningEmissionsV2 + PM10BaselineHaulerTruckIdlingEmissions;
                return c02Total;
            }
        }

        public double TotalPM10EmissionsWithSmashV2
        {
            get
            {
                double c02SmashTotal = PM10SmashingEmissions + PM10SMTRunningEmissions + PM10SMTIdlingEmissions + PM10HaulerRunningEmissionsWithCompactibilityV2 + PM10HaulerIdlingEmissionsWithCompactibility;
                return c02SmashTotal;
            }
        }

        public string PM10PercentSavedV2
        {
            get
            {
                double saved = TotalPM10BaselineTruckEmissionsV2 - TotalPM10EmissionsWithSmashV2;
                double percent = saved / TotalPM10BaselineTruckEmissionsV2;
                string changed = string.Format("{0:P2}", percent);
                return changed;
            }
        }

        // SO2 Emissions

        public double SO2BaselineHaulerTruckRunningEmissionsV2
        {
            get
            {
                Emissions emissions = new Emissions();
                int yearlyHauls = PreSMTYearlyHauls;
                float vmt = (LandfillDist * 2);// + ToHaulerDist + FromHaulerDist;
                //float vmt = 34;
                double emissionFactor = emissions.RunningSO2;
                double conversionFactor = .002204622622;
                double baslineHaulerTruckRunningEmissions = yearlyHauls * vmt * emissionFactor * conversionFactor;
                return baslineHaulerTruckRunningEmissions;
            }
        }

        public double SO2BaselineHaulerTruckIdlingEmissions
        {
            get
            {
                Emissions emissions = new Emissions();
                int yearlyHauls = PreSMTYearlyHauls;
                double vit = 0.7;
                double emissionFactor = emissions.IdleSO2;
                double conversionFactor = .002204622622;
                double baselineHaulerTruckIdlingEmissions = yearlyHauls * vit * emissionFactor * conversionFactor;
                return baselineHaulerTruckIdlingEmissions;

            }
        }
        public double SO2SmashingEmissions
        {
            get
            {
                Emissions emissions = new Emissions();
                int yearlyHauls = PreSMTYearlyHauls;
                double smashingTime = .083333333;
                double emissionFactor = emissions.SmashSO2;
                double conversionFactor = .002204622622;
                double smashingEmissions = yearlyHauls * smashingTime * emissionFactor * conversionFactor;
                return smashingEmissions;

            }
        }
        public double SO2SMTRunningEmissions
        {
            get
            {
                Emissions emissions = new Emissions();
                double yearlyHauls = PreSMTYearlyHauls;
                //double roundTrip = ToClientDist + FromClientDist;
                double roundTrip = 2;
                double emissionFactor = emissions.SmashRunSO2;
                double conversionFactor = .002204622622;
                double runningEmissions = yearlyHauls * roundTrip * emissionFactor * conversionFactor;
                return runningEmissions;
            }
        }

        public double SO2SMTIdlingEmissions
        {
            get
            {
                Emissions emissions = new Emissions();
                double yearlyHauls = PreSMTYearlyHauls;
                double idlingTime = .083333333;
                double emissionFactor = emissions.SmashIdleSO2;
                double conversionFactor = .002204622622;
                double idlingEmissions = yearlyHauls * idlingTime * emissionFactor * conversionFactor;
                return idlingEmissions;
            }
        }

        public double SO2HaulerRunningEmissionsWithCompactibilityV2
        {
            get
            {
                double total = SO2BaselineHaulerTruckRunningEmissionsV2 * CompactibilityValue;
                return total;
            }
        }
        public double SO2HaulerIdlingEmissionsWithCompactibility
        {
            get
            {
                double total = SO2BaselineHaulerTruckIdlingEmissions * CompactibilityValue;
                return total;
            }
        }

        public double TotalSO2BaselineTruckEmissionsV2
        {
            get
            {

                double c02Total = SO2BaselineHaulerTruckRunningEmissionsV2 + SO2BaselineHaulerTruckIdlingEmissions;
                return c02Total;
            }
        }

        public double TotalSO2EmissionsWithSmashV2
        {
            get
            {
                double c02SmashTotal = SO2SmashingEmissions + SO2SMTRunningEmissions + SO2SMTIdlingEmissions + SO2HaulerRunningEmissionsWithCompactibilityV2 + SO2HaulerIdlingEmissionsWithCompactibility;
                return c02SmashTotal;
            }
        }

        public string SO2PercentSavedV2
        {
            get
            {
                double saved = TotalSO2BaselineTruckEmissionsV2 - TotalSO2EmissionsWithSmashV2;
                double percent = saved / TotalSO2BaselineTruckEmissionsV2;
                string changed = string.Format("{0:P2}", percent);
                return changed;
            }
        }

        // CH4 Emissions

        public double CH4BaselineHaulerTruckRunningEmissionsV2
        {
            get
            {
                Emissions emissions = new Emissions();
                int yearlyHauls = PreSMTYearlyHauls;
                float vmt = (LandfillDist * 2);// + ToHaulerDist + FromHaulerDist;
                //float vmt = 34;
                double emissionFactor = emissions.RunningCH4;
                double conversionFactor = .002204622622;
                double baslineHaulerTruckRunningEmissions = yearlyHauls * vmt * emissionFactor * conversionFactor;
                return baslineHaulerTruckRunningEmissions;
            }
        }

        public double CH4BaselineHaulerTruckIdlingEmissions
        {
            get
            {
                Emissions emissions = new Emissions();
                int yearlyHauls = PreSMTYearlyHauls;
                double vit = 0.7;
                double emissionFactor = emissions.IdleCH4;
                double conversionFactor = .002204622622;
                double baselineHaulerTruckIdlingEmissions = yearlyHauls * vit * emissionFactor * conversionFactor;
                return baselineHaulerTruckIdlingEmissions;

            }
        }
        public double CH4SmashingEmissions
        {
            get
            {
                Emissions emissions = new Emissions();
                int yearlyHauls = PreSMTYearlyHauls;
                double smashingTime = .083333333;
                double emissionFactor = emissions.SmashCH4;
                double conversionFactor = .002204622622;
                double smashingEmissions = yearlyHauls * smashingTime * emissionFactor * conversionFactor;
                return smashingEmissions;

            }
        }
        public double CH4SMTRunningEmissions
        {
            get
            {
                Emissions emissions = new Emissions();
                double yearlyHauls = PreSMTYearlyHauls;
                //double roundTrip = ToClientDist + FromClientDist;
                double roundTrip = 2;
                double emissionFactor = emissions.SmashRunCH4;
                double conversionFactor = .002204622622;
                double runningEmissions = yearlyHauls * roundTrip * emissionFactor * conversionFactor;
                return runningEmissions;
            }
        }
        public double CH4SMTIdlingEmissions
        {
            get
            {
                Emissions emissions = new Emissions();
                double yearlyHauls = PreSMTYearlyHauls;
                double idlingTime = .083333333;
                double emissionFactor = emissions.SmashidleCH4;
                double conversionFactor = .002204622622;
                double idlingEmissions = yearlyHauls * idlingTime * emissionFactor * conversionFactor;
                return idlingEmissions;
            }
        }

        public double CH4HaulerRunningEmissionsWithCompactibilityV2
        {
            get
            {
                double total = CH4BaselineHaulerTruckRunningEmissionsV2 * CompactibilityValue;
                return total;
            }
        }
        public double CH4HaulerIdlingEmissionsWithCompactibility
        {
            get
            {
                double total = CH4BaselineHaulerTruckIdlingEmissions * CompactibilityValue;
                return total;
            }
        }

        public double TotalCH4BaselineTruckEmissionsV2
        {
            get
            {

                double c02Total = CH4BaselineHaulerTruckRunningEmissionsV2 + CH4BaselineHaulerTruckIdlingEmissions;
                return c02Total;
            }
        }

        public double TotalCH4EmissionsWithSmashV2
        {
            get
            {
                double c02SmashTotal = CH4SmashingEmissions + CH4SMTRunningEmissions + CH4SMTIdlingEmissions + CH4HaulerRunningEmissionsWithCompactibilityV2 + CH4HaulerIdlingEmissionsWithCompactibility;
                return c02SmashTotal;
            }
        }

        public string CH4PercentSavedV2
        {
            get
            {
                double saved = TotalCH4BaselineTruckEmissionsV2 - TotalCH4EmissionsWithSmashV2;
                double percent = saved / TotalCH4BaselineTruckEmissionsV2;
                string changed = string.Format("{0:P2}", percent);
                return changed;
            }
        }

        // CO Emissions

        public double COBaselineHaulerTruckRunningEmissionsV2
        {
            get
            {
                Emissions emissions = new Emissions();
                int yearlyHauls = PreSMTYearlyHauls;
                float vmt = (LandfillDist * 2);// + ToHaulerDist + FromHaulerDist;
                //float vmt = 34;
                double emissionFactor = emissions.RunningCO;
                double conversionFactor = .002204622622;
                double baslineHaulerTruckRunningEmissions = yearlyHauls * vmt * emissionFactor * conversionFactor;
                return baslineHaulerTruckRunningEmissions;
            }
        }

        public double COBaselineHaulerTruckIdlingEmissions
        {
            get
            {
                Emissions emissions = new Emissions();
                int yearlyHauls = PreSMTYearlyHauls;
                double vit = 0.7;
                double emissionFactor = emissions.IdleCO;
                double conversionFactor = .002204622622;
                double baselineHaulerTruckIdlingEmissions = yearlyHauls * vit * emissionFactor * conversionFactor;
                return baselineHaulerTruckIdlingEmissions;

            }
        }
        public double COSmashingEmissions
        {
            get
            {
                Emissions emissions = new Emissions();
                int yearlyHauls = PreSMTYearlyHauls;
                double smashingTime = .083333333;
                double emissionFactor = emissions.SmashCO;
                double conversionFactor = .002204622622;
                double smashingEmissions = yearlyHauls * smashingTime * emissionFactor * conversionFactor;
                return smashingEmissions;

            }
        }
        public double COSMTRunningEmissions
        {
            get
            {
                Emissions emissions = new Emissions();
                double yearlyHauls = PreSMTYearlyHauls;
                //double roundTrip = ToClientDist + FromClientDist;
                double roundTrip = 2;
                double emissionFactor = emissions.SmashRunCO;
                double conversionFactor = .002204622622;
                double runningEmissions = yearlyHauls * roundTrip * emissionFactor * conversionFactor;
                return runningEmissions;
            }
        }

        public double COSMTIdlingEmissions
        {
            get
            {
                Emissions emissions = new Emissions();
                double yearlyHauls = PreSMTYearlyHauls;
                double idlingTime = .083333333;
                double emissionFactor = emissions.SmashIdleCO;
                double conversionFactor = .002204622622;
                double idlingEmissions = yearlyHauls * idlingTime * emissionFactor * conversionFactor;
                return idlingEmissions;
            }
        }

        public double COHaulerRunningEmissionsWithCompactibilityV2
        {
            get
            {
                double total = COBaselineHaulerTruckRunningEmissionsV2 * CompactibilityValue;
                return total;
            }
        }
        public double COHaulerIdlingEmissionsWithCompactibility
        {
            get
            {
                double total = COBaselineHaulerTruckIdlingEmissions * CompactibilityValue;
                return total;
            }
        }

        public double TotalCOBaselineTruckEmissionsV3
        {
            get
            {

                double c02Total = COBaselineHaulerTruckRunningEmissionsV2 + COBaselineHaulerTruckIdlingEmissions;
                return c02Total;
            }
        }

        public double TotalCOEmissionsWithSmashV3
        {
            get
            {
                double c02SmashTotal = COSmashingEmissions + COSMTRunningEmissions + COSMTIdlingEmissions + COHaulerRunningEmissionsWithCompactibilityV2 + COHaulerIdlingEmissionsWithCompactibility;
                return c02SmashTotal;
            }
        }

        public string COPercentSavedV3
        {
            get
            {
                double saved = TotalCOBaselineTruckEmissionsV3 - TotalCOEmissionsWithSmashV3;
                double percent = saved / TotalCOBaselineTruckEmissionsV3;
                string changed = string.Format("{0:P2}", percent);
                return changed;
            }
        }

        // VOC Emissions

        public double VOCBaselineHaulerTruckRunningEmissionsV2
        {
            get
            {
                Emissions emissions = new Emissions();
                int yearlyHauls = PreSMTYearlyHauls;
                float vmt = (LandfillDist * 2);// + ToHaulerDist + FromHaulerDist;
                //float vmt = 34;
                double emissionFactor = emissions.RunningVOC;
                double conversionFactor = .002204622622;
                double baslineHaulerTruckRunningEmissions = yearlyHauls * vmt * emissionFactor * conversionFactor;
                return baslineHaulerTruckRunningEmissions;
            }
        }

        public double VOCBaselineHaulerTruckIdlingEmissions
        {
            get
            {
                Emissions emissions = new Emissions();
                int yearlyHauls = PreSMTYearlyHauls;
                double vit = 0.7;
                double emissionFactor = emissions.IdleVOC;
                double conversionFactor = .002204622622;
                double baselineHaulerTruckIdlingEmissions = yearlyHauls * vit * emissionFactor * conversionFactor;
                return baselineHaulerTruckIdlingEmissions;

            }
        }
        public double VOCSmashingEmissions
        {
            get
            {
                Emissions emissions = new Emissions();
                int yearlyHauls = PreSMTYearlyHauls;
                double smashingTime = .083333333;
                double emissionFactor = emissions.SmashVOC;
                double conversionFactor = .002204622622;
                double smashingEmissions = yearlyHauls * smashingTime * emissionFactor * conversionFactor;
                return smashingEmissions;

            }
        }
        public double VOCSMTRunningEmissions
        {
            get
            {
                Emissions emissions = new Emissions();
                double yearlyHauls = PreSMTYearlyHauls;
                //double roundTrip = ToClientDist + FromClientDist;
                double roundTrip = 2;
                double emissionFactor = emissions.SmashRunVOC;
                double conversionFactor = .002204622622;
                double runningEmissions = yearlyHauls * roundTrip * emissionFactor * conversionFactor;
                return runningEmissions;
            }
        }
        public double VOCSMTIdlingEmissions
        {
            get
            {
                Emissions emissions = new Emissions();
                double yearlyHauls = PreSMTYearlyHauls;
                double idlingTime = .083333333;
                double emissionFactor = emissions.SmashIdleVOC;
                double conversionFactor = .002204622622;
                double idlingEmissions = yearlyHauls * idlingTime * emissionFactor * conversionFactor;
                return idlingEmissions;
            }
        }

        public double VOCHaulerRunningEmissionsWithCompactibilityV2
        {
            get
            {
                double total = VOCBaselineHaulerTruckRunningEmissionsV2 * CompactibilityValue;
                return total;
            }
        }
        public double VOCHaulerIdlingEmissionsWithCompactibility
        {
            get
            {
                double total = VOCBaselineHaulerTruckIdlingEmissions * CompactibilityValue;
                return total;
            }
        }

        public double TotalVOCBaselineTruckEmissionsV2
        {
            get
            {

                double c02Total = VOCBaselineHaulerTruckRunningEmissionsV2 + VOCBaselineHaulerTruckIdlingEmissions;
                return c02Total;
            }
        }

        public double TotalVOCEmissionsWithSmashV2
        {
            get
            {
                double c02SmashTotal = VOCSmashingEmissions + VOCSMTRunningEmissions + VOCSMTIdlingEmissions + VOCHaulerRunningEmissionsWithCompactibilityV2 + VOCHaulerIdlingEmissionsWithCompactibility;
                return c02SmashTotal;
            }
        }

        public string VOCPercentSavedV2
        {
            get
            {
                double saved = TotalVOCBaselineTruckEmissionsV2 - TotalVOCEmissionsWithSmashV2;
                double percent = saved / TotalVOCBaselineTruckEmissionsV2;
                string changed = string.Format("{0:P2}", percent);
                return changed;
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

        // CO2 eq Emissions

        public double CO2EQBaselineHaulerTruckRunningEmissionsV2
        {
            get
            {
                Emissions emissions = new Emissions();
                int yearlyHauls = PreSMTYearlyHauls;
                float vmt = (LandfillDist * 2);// + ToHaulerDist + FromHaulerDist;
                //float vmt = 34;
                double emissionFactor = emissions.RunningCO2EQ;
                double conversionFactor = .002204622622;
                double baslineHaulerTruckRunningEmissions = yearlyHauls * vmt * emissionFactor * conversionFactor;
                return baslineHaulerTruckRunningEmissions;
            }
        }

        public double CO2EQBaselineHaulerTruckIdlingEmissions
        {
            get
            {
                Emissions emissions = new Emissions();
                int yearlyHauls = PreSMTYearlyHauls;
                double vit = 0.7;
                double emissionFactor = emissions.IdleCO2EQ;
                double conversionFactor = .002204622622;
                double baselineHaulerTruckIdlingEmissions = yearlyHauls * vit * emissionFactor * conversionFactor;
                return baselineHaulerTruckIdlingEmissions;

            }
        }
        public double CO2EQSmashingEmissions
        {
            get
            {
                Emissions emissions = new Emissions();
                int yearlyHauls = PreSMTYearlyHauls;
                double smashingTime = .083333333;
                double emissionFactor = emissions.SmashCO2EQ;
                double conversionFactor = .002204622622;
                double smashingEmissions = yearlyHauls * smashingTime * emissionFactor * conversionFactor;
                return smashingEmissions;

            }
        }

        public double CO2EQSMTRunningEmissions
        {
            get
            {
                Emissions emissions = new Emissions();
                double yearlyHauls = PreSMTYearlyHauls;
                //double roundTrip = ToClientDist + FromClientDist;
                double roundTrip = 2;
                double emissionFactor = emissions.SmashRunCO2EQ;
                double conversionFactor = .002204622622;
                double runningEmissions = yearlyHauls * roundTrip * emissionFactor * conversionFactor;
                return runningEmissions;
            }
        }

        public double CO2EQSMTIdlingEmissions
        {
            get
            {
                Emissions emissions = new Emissions();
                double yearlyHauls = PreSMTYearlyHauls;
                double idlingTime = .083333333;
                double emissionFactor = emissions.SmashIdleC02EQ;
                double conversionFactor = .002204622622;
                double idlingEmissions = yearlyHauls * idlingTime * emissionFactor * conversionFactor;
                return idlingEmissions;
            }
        }

        public double CO2EQHaulerRunningEmissionsWithCompactibilityV2
        {
            get
            {
                double total = CO2EQBaselineHaulerTruckRunningEmissionsV2 * CompactibilityValue;
                return total;
            }
        }
        public double CO2EQHaulerIdlingEmissionsWithCompactibility
        {
            get
            {
                double total = CO2EQBaselineHaulerTruckIdlingEmissions * CompactibilityValue;
                return total;
            }
        }

        public double TotalCO2EQBaselineTruckEmissionsV2
        {
            get
            {

                double c02Total = CO2EQBaselineHaulerTruckRunningEmissionsV2 + CO2EQBaselineHaulerTruckIdlingEmissions;
                return c02Total;
            }
        }

        public double TotalCO2EQEmissionsWithSmashV2
        {
            get
            {
                double c02SmashTotal = CO2EQSmashingEmissions + CO2EQSMTRunningEmissions + CO2EQSMTIdlingEmissions + CO2EQHaulerRunningEmissionsWithCompactibilityV2 + CO2EQHaulerIdlingEmissionsWithCompactibility;
                return c02SmashTotal;
            }
        }

        public double TotalCO2EQSavedV2
        {
            get
            {
                double total = TotalCO2BaselineTruckEmissionsV2 - TotalCO2EmissionsWithSmashV2;
                return total;
            }
        }

        public string CO2EQPercentSavedV2
        {
            get
            {
                double percent = TotalCO2EQSavedV2 / TotalCO2BaselineTruckEmissionsV2;
                string changed = string.Format("{0:P2}", percent);
                return changed;
            }
        }


    }
}