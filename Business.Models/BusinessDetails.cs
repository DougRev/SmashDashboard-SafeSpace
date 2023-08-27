using BusinessData;
using BusinessData.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BusinessModels
{
    public class BusinessDetails
    {
        [Display(Name = "Client ID")]
        public int BusinessId { get; set; }

        [Display(Name = "Client Name")]
        public string BusinessName { get; set; }
        [Display(Name = "Facility ID")]
        public string FacilityID { get; set; }
        public State State { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        [Display(Name = "Zip Code")]
        [Range(1, 99950, ErrorMessage = "Zip code must be between 00001 and 99950.")]
        public string ZipCode { get; set; }
        public string ServiceLocation { get; set; }

        public Guid OwnerId { get; set; }

        //public virtual Franchisee Franchisee { get; set; }
        public int FranchiseId { get; set; }
        [Display(Name = "Franchise Name")]
        public string FranchiseName { get; set; }
        public int? AccountId { get; set; }
        [Display(Name = "National Account")]

        public string AccountName { get; set; }
        public int FranchiseeId { get; set; }
        public string FranchiseeName { get; set; }
        public Compactibility Compactibility { get; set; }
        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

        [Display(Name = "Transfer Station")]
        public bool XferStation { get; set; }

        [Display(Name = "SMT Distance to Client")]
        public float ToClientDist { get; set; }

        [Display(Name = "SMT Distance from Client")]
        public float FromClientDist { get; set; }

        [Display(Name = "Hauler Distance to Client")]
        public float ToHaulerDist { get; set; }

        [Display(Name = "Hauler Distance from Client")]
        public float FromHaulerDist { get; set; }

        [Display(Name = "Distance to Landfill (One Way)")]
        public float LandfillDist { get; set; }

        [Display(Name = "Total Hauls Per Week")]
        public float HaulsPerWeek { get; set; }

        [Display(Name = "Number of Dumpsters")]
        public float NumberOfDumpsters { get; set; }

        [Display(Name = "Est. Yearly Hauls ")]
        public float PreSMTYearlyHauls { get; set; }

       
        public double AllEmissionsBaselineTotalsV2 { get; set; }
        public double AllEmissionsWithSmashTotalsV2 { get; set; }
        public double AllEmissionsSavedWithSmashV2 { get; set; }
        public string AllSavingsTotalV2 { get; set; }

        //public double FourDayTotalBaseline { get { return CalculateFourDayEmissions(AllEmissionsBaselineTotalsV2); } }
        //public double FourDayTotalWithSmash { get { return CalculateFourDayEmissions(AllEmissionsWithSmashTotalsV2); } }
        //public double FourDayTotalSaved { get { return CalculateFourDayEmissions(AllEmissionsSavedWithSmashV2); } }
        //public string FourDaySavingsTotal
        //{
        //    get
        //    {
        //        if (FourDayTotalBaseline > 0)
        //        {
        //            double savingsPercent = (FourDayTotalSaved / FourDayTotalBaseline) * 100;
        //            return savingsPercent.ToString("N2") + "%";
        //        }
        //        else
        //        {
        //            return "Invalid value";
        //        }
        //    }
        //}

        //public string CarsRemoved
        //{
        //    get
        //    {
        //        double carsRemoved = FourDayTotalSaved / (4.6 * 2204.62 / 365); // convert metric tons to pounds and per year to per day
        //        return carsRemoved.ToString("N2");
        //    }
        //}

        //public string TreesPlanted
        //{
        //    get
        //    {
        //        double treesPlanted = FourDayTotalSaved / (48 / 365); // per year to per day
        //        return treesPlanted.ToString("N2");
        //    }
        //}

        //public string GallonsOfGasolineSaved
        //{
        //    get
        //    {
        //        double gallonsSaved = FourDayTotalSaved / 19.6;
        //        return gallonsSaved.ToString("N2");
        //    }
        //}

        //public string EnergySavedKWh
        //{
        //    get
        //    {
        //        double energySaved = FourDayTotalSaved / 1.222;
        //        return energySaved.ToString("N2");
        //    }
        //}

        //public string DistanceTravelled
        //{
        //    get
        //    {
        //        double distanceTravelled = FourDayTotalSaved / (404 / 2204.62); // grams to pounds
        //        return distanceTravelled.ToString("N2");
        //    }
        //}





        // NOX
        //public double FourDayTotalNOXEmissionsV2 { get { return CalculateFourDayEmissions(TotalNOXBaselineTruckEmissionsV2); } }
        //public double FourDayTotalNOXEmissionsWithSmashV2 { get { return CalculateFourDayEmissions(TotalNOXEmissionsWithSmashV2); } }

        //// N2O
        //public double FourDayTotalN2OEmissionsV2 { get { return CalculateFourDayEmissions(TotalN20BaselineTruckEmissionsV2); } }
        //public double FourDayTotalN2OEmissionsWithSmashV2 { get { return CalculateFourDayEmissions(TotalN20EmissionsWithSmashV2); } }

        //// PM2.5
        //public double FourDayTotalPM25EmissionsV2 { get { return CalculateFourDayEmissions(TotalPM25BaselineTruckEmissionsV2); } }
        //public double FourDayTotalPM25EmissionsWithSmashV2 { get { return CalculateFourDayEmissions(TotalPM25EmissionsWithSmashV2); } }

        //// PM10
        //public double FourDayTotalPM10EmissionsV2 { get { return CalculateFourDayEmissions(TotalPM10BaselineTruckEmissionsV2); } }
        //public double FourDayTotalPM10EmissionsWithSmashV2 { get { return CalculateFourDayEmissions(TotalPM10EmissionsWithSmashV2); } }

        //// SO2
        //public double FourDayTotalSO2EmissionsV2 { get { return CalculateFourDayEmissions(TotalSO2BaselineTruckEmissionsV2); } }
        //public double FourDayTotalSO2EmissionsWithSmashV2 { get { return CalculateFourDayEmissions(TotalSO2EmissionsWithSmashV2); } }

        //// CH4
        //public double FourDayTotalCH4EmissionsV2 { get { return CalculateFourDayEmissions(TotalCH4BaselineTruckEmissionsV2); } }
        //public double FourDayTotalCH4EmissionsWithSmashV2 { get { return CalculateFourDayEmissions(TotalCH4EmissionsWithSmashV2); } }

        //// CO
        //public double FourDayTotalCOEmissionsV2 { get { return CalculateFourDayEmissions(TotalCOBaselineTruckEmissionsV3); } }
        //public double FourDayTotalCOEmissionsWithSmashV2 { get { return CalculateFourDayEmissions(TotalCOEmissionsWithSmashV3); } }

        //// VOC
        //public double FourDayTotalVOCEmissionsV2 { get { return CalculateFourDayEmissions(TotalVOCBaselineTruckEmissionsV2); } }
        //public double FourDayTotalVOCEmissionsWithSmashV2 { get { return CalculateFourDayEmissions(TotalVOCEmissionsWithSmashV2); } }

        //// CO2
        //public double FourDayTotalCO2EmissionsV2 { get { return CalculateFourDayEmissions(TotalCO2BaselineTruckEmissionsV2); } }
        //public double FourDayTotalCO2EmissionsWithSmashV2 { get { return CalculateFourDayEmissions(TotalCO2EmissionsWithSmashV2); } }

        //public double FourDayCO2Saved { get { return CalculateFourDayEmissions(TotalCO2SavedV2); } }


        //private double CalculateFourDayEmissions(double yearlyEmissions)
        //{
        //    return (yearlyEmissions / 365) * 4;
        //}



        //NOX

        /*[Display(Name = "NOX Hauler Running Emissions")]
        public double NOXBaselineHaulerTruckRunningEmissions { get; set; }

        [Display(Name = "NOX Hauler Idling Emissions")]
        public double NOXBaselineHaulerTruckIdlingEmissions { get; set; }

        [Display(Name = "NOX SMT Smashing Emissions")]
        public double NOXSmashingEmissions { get; set; }

        [Display(Name = "NOX SMT Running Emissions")]
        public double NOXSMTRunningEmissions { get; set; }

        [Display(Name = "NOX SMT Idling Emissions")]
        public double NOXSMTIdlingEmissions { get; set; }

        [Display(Name = "NOX Hauler Running Emissions w/ Smash Services")]
        public double NOXHaulerRunningEmissionsWithCompactibility { get; set; }

        [Display(Name = "NOX Hauler Idling Emissions w/ Smash Services")]
        public double NOXHaulerIdlingEmissionsWithCompactibility { get; set; }*/

        [Display(Name = "NOX Total Hauler Emissions")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double TotalNOXBaselineTruckEmissionsV2 { get; set; }

        [Display(Name = "NOX Total SMT Emissions")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double TotalNOXEmissionsWithSmashV2 { get; set; }
        
        [Display(Name = "NOX Percent Saved using SMT")]
        public string NOXPercentSavedV2 { get; set; }

        //N20

        /*[Display(Name = "N20Hauler Running Emissions")]
        public double N20BaselineHaulerTruckRunningEmissions { get; set; }

        [Display(Name = "N20Hauler Idling Emissions")]
        public double N20BaselineHaulerTruckIdlingEmissions { get; set; }

        [Display(Name = "N20 SMT Smashing Emissions")]
        public double N20SmashingEmissions { get; set; }

        [Display(Name = "N20 SMT Running Emissions")]
        public double N20SMTRunningEmissions { get; set; }

        [Display(Name = "N20 SMT Idling Emissions")]
        public double N20SMTIdlingEmissions { get; set; }

        [Display(Name = "N20 Hauler Running Emissions w/ Smash Services")]
        public double N20HaulerRunningEmissionsWithCompactibility { get; set; }

        [Display(Name = "N20 Hauler Idling Emissions w/ Smash Services")]
        public double N20HaulerIdlingEmissionsWithCompactibility { get; set; }*/
        
        [Display(Name = "N20 Total Hauler Emissions")]
        public double TotalN20BaselineTruckEmissionsV2 { get; set; }
        
        [Display(Name = "N20 Total SMT Emissions")]
        public double TotalN20EmissionsWithSmashV2 { get; set; }

        [Display(Name = "N20 Percent Saved using SMT")]
        public string N20PercentSavedV2 { get; set; }

        //PM2.5

        /*[Display(Name = "PM2.5Hauler Running Emissions")]
        public double PM25BaselineHaulerTruckRunningEmissions { get; set; }

        [Display(Name = "PM2.5Hauler Idling Emissions")]
        public double PM25BaselineHaulerTruckIdlingEmissions { get; set; }

        [Display(Name = "PM2.5 SMT Smashing Emissions")]
        public double PM25SmashingEmissions { get; set; }

        [Display(Name = "PM2.5 SMT Running Emissions")]
        public double PM25SMTRunningEmissions { get; set; }

        [Display(Name = "PM2.5 SMT Idling Emissions")]
        public double PM25SMTIdlingEmissions { get; set; }

        [Display(Name = "PM2.5 Hauler Running Emissions w/ Smash Services")]
        public double PM25HaulerRunningEmissionsWithCompactibility { get; set; }

        [Display(Name = "PM2.5 Hauler Idling Emissions w/ Smash Services")]
        public double PM25HaulerIdlingEmissionsWithCompactibility { get; set; }*/
        
        [Display(Name = "PM2.5 Total Hauler Emissions")]
        public double TotalPM25BaselineTruckEmissionsV2 { get; set; }
        
        [Display(Name = "PM10 Total SMT Emissions")]
        public double TotalPM25EmissionsWithSmashV2 { get; set; }

        [Display(Name = "PM2.5 Percent Saved using SMT")]
        public string PM25PercentSavedV2 { get; set; }

        //PM2.5 Emissions

        /*[Display(Name = "PM10 Hauler Running Emissions")]
        public double PM10BaselineHaulerTruckRunningEmissions { get; set; }

        [Display(Name = "PM10 Hauler Idling Emissions")]
        public double PM10BaselineHaulerTruckIdlingEmissions { get; set; }

        [Display(Name = "PM10 SMT Smashing Emissions")]
        public double PM10SmashingEmissions { get; set; }

        [Display(Name = "PM10 SMT Running Emissions")]
        public double PM10SMTRunningEmissions { get; set; }

        [Display(Name = "PM10 SMT Idling Emissions")]
        public double PM10SMTIdlingEmissions { get; set; }

        [Display(Name = "PM10 Hauler Running Emissions w/ Smash Services")]
        public double PM10HaulerRunningEmissionsWithCompactibility { get; set; }

        [Display(Name = "PM10 Hauler Idling Emissions w/ Smash Services")]
        public double PM10HaulerIdlingEmissionsWithCompactibility { get; set; }*/

        [Display(Name = "PM10 Total Hauler Emissions")]
        public double TotalPM10BaselineTruckEmissionsV2 { get; set; }

        [Display(Name = "PM10 Total SMT Emissions")]
        public double TotalPM10EmissionsWithSmashV2 { get; set; }

        [Display(Name = "PM10 Percent Saved using SMT")]
        public string PM10PercentSavedV2 { get; set; }

        //PM10 Emissions


        /*[Display(Name = "SO2 Hauler Running Emissions")]
        public double SO2BaselineHaulerTruckRunningEmissions { get; set; }

        [Display(Name = "SO2 Hauler Idling Emissions")]
        public double SO2BaselineHaulerTruckIdlingEmissions { get; set; }

        [Display(Name = "SO2 SMT Smashing Emissions")]
        public double SO2SmashingEmissions { get; set; }

        [Display(Name = "SO2 SMT Running Emissions")]
        public double SO2SMTRunningEmissions { get; set; }

        [Display(Name = "SO2 SMT Idling Emissions")]
        public double SO2SMTIdlingEmissions { get; set; }

        [Display(Name = "SO2 Hauler Running Emissions w/ Smash Services")]
        public double SO2HaulerRunningEmissionsWithCompactibility { get; set; }

        [Display(Name = "SO2 Hauler Running Emissions w/ Smash Services")]
        public double SO2HaulerIdlingEmissionsWithCompactibility { get; set; }*/

        [Display(Name = "SO2 Total Hauler Emissions")]
        public double TotalSO2BaselineTruckEmissionsV2 { get; set; }

        [Display(Name = "SO2 Total SMT Emissions")]
        public double TotalSO2EmissionsWithSmashV2 { get; set; }

        [Display(Name = "SO2 Percent Saved using SMT")]
        public string SO2PercentSavedV2 { get; set; }

        //CH4 Emissions

        /*[Display(Name = "CH4 Hauler Running Emissions")]
        public double CH4BaselineHaulerTruckRunningEmissions { get; set; }

        [Display(Name = "CH4 Hauler Idling Emissions")]
        public double CH4BaselineHaulerTruckIdlingEmissions { get; set; }

        [Display(Name = "CH4 SMT Smashing Emissions")]
        public double CH4SmashingEmissions { get; set; }

        [Display(Name = "CH4 SMT Running Emissions")]
        public double CH4SMTRunningEmissions { get; set; }

        [Display(Name = "CH4 SMT Idling Emissions")]
        public double CH4SMTIdlingEmissions { get; set; }

        [Display(Name = "CH4 Hauler Running Emissions w/ Smash Services")]
        public double CH4HaulerRunningEmissionsWithCompactibility { get; set; }

        [Display(Name = "CH4 Hauler Idling Emissions w/ Smash Services")]
        public double CH4HaulerIdlingEmissionsWithCompactibility { get; set; }*/

        [Display(Name = "CH4 Total Hauler Emissions")]
        public double TotalCH4BaselineTruckEmissionsV2 { get; set; }

        [Display(Name = "CH4 Total SMT Emissions")]
        public double TotalCH4EmissionsWithSmashV2 { get; set; }

        [Display(Name = "CH4 Percent Saved using SMT")]
        public string CH4PercentSavedV2 { get; set; }

        // CO Emissions

        /*[Display(Name = "CO Hauler Running Emissions")]
        public double COBaselineHaulerTruckRunningEmissions { get; set; }

        [Display(Name = "COHauler Idling Emissions")]
        public double COBaselineHaulerTruckIdlingEmissions { get; set; }

        [Display(Name = "CO SMT Smashing Emissions")]
        public double COSmashingEmissions { get; set; }

        [Display(Name = "CO SMT Running Emissions")]
        public double COSMTRunningEmissions { get; set; }

        [Display(Name = "CO SMT Idling Emissions")]
        public double COSMTIdlingEmissions { get; set; }

        [Display(Name = "CO Hauler Running Emissions w/ Smash Services")]
        public double COHaulerRunningEmissionsWithCompactibility { get; set; }

        [Display(Name = "CO Hauler Idling Emissions w/ Smash Services")]
        public double COHaulerIdlingEmissionsWithCompactibility { get; set; }*/

        [Display(Name = "CO Total Hauler Emissions")]
        public double TotalCOBaselineTruckEmissionsV3 { get; set; }

        [Display(Name = "CO Total SMT Emissions")]
        public double TotalCOEmissionsWithSmashV3 { get; set; }

        [Display(Name = "CO Percent Saved using SMT")]
        public string COPercentSavedV3 { get; set; }

        //VOC Emissions

        /*[Display(Name = "VOC Hauler Running Emissions")]
        public double VOCBaselineHaulerTruckRunningEmissions { get; set; }

        [Display(Name = "VOC Hauler Idling Emissions")]
        public double VOCBaselineHaulerTruckIdlingEmissions { get; set; }

        [Display(Name = "VOC SMT Smashing Emissions")]
        public double VOCSmashingEmissions { get; set; }

        [Display(Name = "VOC SMT Running Emissions")]
        public double VOCSMTRunningEmissions { get; set; }

        [Display(Name = "VOC SMT Idling Emissions")]
        public double VOCSMTIdlingEmissions { get; set; }

        [Display(Name = "VOC Hauler Running Emissions w/ Smash Services")]
        public double VOCHaulerRunningEmissionsWithCompactibility { get; set; }

        [Display(Name = "VOC Hauler Idling Emissions w/ Smash Services")]
        public double VOCHaulerIdlingEmissionsWithCompactibility { get; set; }*/

        [Display(Name = "VOC Total Hauler Emissions")]
        public double TotalVOCBaselineTruckEmissionsV2 { get; set; }

        [Display(Name = "VOC Total Saved Using SMT")]
        public double TotalVOCEmissionsWithSmashV2 { get; set; }

        [Display(Name = "VOC Total SMT Emissions")]
        public string VOCPercentSavedV2 { get; set; }

        //CO2 Emissions

        /*[Display(Name = "CO2 Hauler Running Emissions")]
        public double CO2BaselineHaulerTruckRunningEmissions { get; set; }

        [Display(Name = "CO2 Hauler Idling Emissions")]
        public double CO2BaselineHaulerTruckIdlingEmissions { get; set; }

        [Display(Name = "CO2 SMT Smashing Emissions")]
        public double CO2SmashingEmissions { get; set; }

        [Display(Name = "CO2 SMT Running Emissions")]
        public double CO2SMTRunningEmissions { get; set; }

        [Display(Name = "CO2 SMT Idling Emissions")]
        public double CO2SMTIdlingEmissions { get; set; }

        [Display(Name = "CO2 Hauler Running Emissions w/ Smash Services")]
        public double CO2HaulerRunningEmissionsWithCompactibility { get; set; }

        [Display(Name = "CO2 Hauler Idling Emissions w/ Smash Services")]
        public double CO2HaulerIdlingEmissionsWithCompactibility { get; set; }*/

        [Display(Name = "CO2 Total Hauler Emissions")]
        public double TotalCO2BaselineTruckEmissionsV2 { get; set; }

        [Display(Name = "CO2 Total SMT Emissions")]
        public double TotalCO2EmissionsWithSmashV2 { get; set; }

        [Display(Name = "Total CO2 Saved")]
        public double TotalCO2SavedV2 { get; set; }

        [Display(Name = "CO2 Percent Saved using SMT")]
        public string CO2PercentSavedV2 { get; set; }


        public double BaselineTotals { get; set; }
        public double EmissionsWithSmashTotals { get; set; }
        public string SavingsTotals { get; set; }


        /* This is CO2EQ not sure this will be used at all
         * 
        [Display(Name = "Hauler Running Emissions")]
        public double CO2EQBaselineHaulerTruckRunningEmissions { get; set; }

        [Display(Name = "Hauler Idling Emissions")]
        public double CO2EQBaselineHaulerTruckIdlingEmissions { get; set; }
        public double CO2EQSmashingEmissions { get; set; }
        public double CO2EQSMTRunningEmissions { get; set; }
        public double CO2EQSMTIdlingEmissions { get; set; }
        public double CO2EQHaulerRunningEmissionsWithCompactibility { get; set; }
        public double CO2EQHaulerIdlingEmissionsWithCompactibility { get; set; }
        public double TotalCO2EQBaselineTruckEmissions { get; set; }
        public double TotalCO2EQEmissionsWithSmash { get; set; }
        public string CO2EQPercentSaved { get; set; }*/

    }
}
