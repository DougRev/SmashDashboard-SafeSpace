﻿using BusinessData;
using BusinessData.Enum;
using BusinessData.Interfaces;
using BusinessModels;
using BusinessShared;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using OpenQA.Selenium.DevTools;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessServices
{
    public class ClientService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserIdProvider _userIdProvider;
        private InvoiceService _invoiceService;


        public ClientService(ApplicationDbContext context, IUserIdProvider userIdProvider)
        {
            _context = context;
            _userIdProvider = userIdProvider;
            _invoiceService = new InvoiceService(context);
        }

        public QuoteResultViewModel PerformCalculations(BusinessCreate model)
        {
            // Perform calculations here and return the results
            string businessName = model.BusinessName;
            float yearlyHauls = model.PreSMTYearlyHauls;
            double haulerEm = model.TotalCO2BaselineTruckEmissionsV2;
            double smtEm = model.TotalCO2EmissionsWithSmashV2;
            double co2 = model.TotalCO2SavedV2;
            string co2Percent = model.CO2PercentSavedV2;
            float landfillDist = model.LandfillDist;
            float hauls = model.HaulsWithSMT;

            var quoteResult = new QuoteResultViewModel
            {
                BusinessName = businessName,
                PreSMTYearlyHauls = yearlyHauls,
                TotalCO2BaselineTruckEmissionsV2 = haulerEm,
                TotalCO2EmissionsWithSmashV2 = smtEm,
                TotalCO2SavedV2 = co2,
                CO2PercentSavedV2 = co2Percent,
                LandfillDist = landfillDist,
                HaulsWithSMT = hauls,
            };

            return quoteResult;
        }




        public bool CreateBusiness(BusinessCreate model, bool saveToDatabase = true)
        {
            var entity = new Client()
            {
                OwnerId = _userIdProvider.GetUserId(),
                BusinessId = model.BusinessId ?? 0,
                BusinessName = model.BusinessName,
                FacilityID = model.FacilityID,
                State = model.State,
                City = model.City,
                Address = model.Address,
                ZipCode = model.ZipCode,
                FranchiseId = model.FranchiseId,
                AccountId = model.AccountId,
                HaulsPerWeek = model.HaulsPerWeek,
                NumberOfDumpsters = model.NumberOfDumpsters,
                LandfillDist = model.LandfillDist,
                Compactibility = model.Compactibility,
                AddToDb = model.AddToDb,
                //FranchiseeId = model.FranchiseeId,
            };

            using (var ctx = new ApplicationDbContext())
            {
                ctx.Clients.Add(entity);

                // If saveToDatabase is true, save the changes, otherwise, return true without saving
                return saveToDatabase ? ctx.SaveChanges() == 1 : true;
            }
        }



        public IEnumerable<BusinessListItem> GetBusinesses()
        {
            using (var ctx = new ApplicationDbContext())
            {
                var query = ctx.Clients
                    .OrderBy(e => e.BusinessId) // or any other property
                    .Select(e => new BusinessListItem
                    {
                        BusinessId = e.BusinessId,
                        BusinessName = e.BusinessName,
                        FacilityID = e.FacilityID,
                        Address = e.Address,
                        City = e.City,
                        ServiceLocation = e.ServiceLocation,
                        FranchiseName = e.Franchise.FranchiseName,
                        AccountName = e.NationalAccount.AccountName,
                    });
                return query.ToArray();
            }
        }




        public BusinessDetails GetBusinessById(int businessId)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity =
                ctx
                .Clients
                .Include(e => e.Franchise) //Eager loading for Franchise
                .Single(e => e.BusinessId == businessId);

                // Fetch invoices based on service location of the business
                var invoices = _invoiceService.GetInvoicesByVonigoClientId(entity.VonigoClientId);

                return
                new BusinessDetails
                {
                    BusinessId = entity.BusinessId,
                    BusinessName = entity.BusinessName,
                    FacilityID = entity.FacilityID,
                    State = entity.State,
                    City = entity.City,
                    Address = entity.Address,
                    ZipCode = entity.ZipCode,
                    Invoices = invoices,
                    //FranchiseeId = entity.FranchiseeId,
                    //FranchiseeName = entity.Franchisee.FranchiseeName,
                    FranchiseId = entity.Franchise.FranchiseId,
                    FranchiseName = entity.Franchise.FranchiseName,
                    AccountId = entity.NationalAccount?.AccountId, //Null check before accessing property
                    AccountName = entity.NationalAccount?.AccountName, //Null check before accessing property
                    Compactibility = entity.Compactibility,
                    ToClientDist = entity.ToClientDist,
                    FromClientDist = entity.FromClientDist,
                    ToHaulerDist = entity.ToHaulerDist,
                    FromHaulerDist = entity.FromHaulerDist,
                    LandfillDist = entity.LandfillDist,
                    HaulsPerWeek = entity.HaulsPerWeek,
                    NumberOfDumpsters = entity.NumberOfDumpsters,
                    PreSMTYearlyHauls = entity.PreSMTYearlyHauls,
                    TotalCO2SavedV2 = entity.TotalCO2SavedV2,

                    AllEmissionsBaselineTotalsV2 = entity.AllEmissionsBaselineTotalsV2,
                    AllEmissionsWithSmashTotalsV2 = entity.AllEmissionsWithSmashTotalsV2,
                    AllEmissionsSavedWithSmashV2 = entity.AllEmissionsSavedWithSmashV2,
                    AllSavingsTotalV2 = entity.AllSavingsTotalV2,

                    /*NOXBaselineHaulerTruckRunningEmissions = entity.NOXBaselineHaulerTruckRunningEmissionsV2,
                    NOXBaselineHaulerTruckIdlingEmissions = entity.NOXBaselineHaulerTruckIdlingEmissions,
                    NOXSmashingEmissions = entity.NOXSmashingEmissions,
                    NOXSMTRunningEmissions = entity.NOXSMTRunningEmissions,
                    NOXSMTIdlingEmissions = entity.NOXSMTIdlingEmissions,
                    NOXHaulerRunningEmissionsWithCompactibility = entity.NOXHaulerRunningEmissionsWithCompactibilityV2,
                    NOXHaulerIdlingEmissionsWithCompactibility = entity.NOXHaulerIdlingEmissionsWithCompactibility,*/
                    TotalNOXBaselineTruckEmissionsV2 = entity.TotalNOXBaselineTruckEmissionsV2,
                    TotalNOXEmissionsWithSmashV2 = entity.TotalNOXEmissionsWithSmashV2,
                    NOXPercentSavedV2 = entity.NOXPercentSavedV2,

                    /*N20BaselineHaulerTruckRunningEmissions = entity.N20BaselineHaulerTruckRunningEmissionsV2,
                    N20BaselineHaulerTruckIdlingEmissions= entity.N20BaselineHaulerTruckIdlingEmissions,
                    N20SmashingEmissions= entity.N20SmashingEmissions,
                    N20SMTRunningEmissions= entity.N20SMTRunningEmissions,
                    N20SMTIdlingEmissions= entity.N20SMTIdlingEmissions,
                    N20HaulerRunningEmissionsWithCompactibility= entity.N20HaulerRunningEmissionsWithCompactibilityV2,
                    N20HaulerIdlingEmissionsWithCompactibility= entity.N20HaulerIdlingEmissionsWithCompactibility,*/
                    TotalN20BaselineTruckEmissionsV2 = entity.TotalN20BaselineTruckEmissionsV2,
                    TotalN20EmissionsWithSmashV2 = entity.TotalN20EmissionsWithSmashV2,
                    N20PercentSavedV2 = entity.N20PercentSavedV2,

                    /*PM25BaselineHaulerTruckRunningEmissions = entity.PM25BaselineHaulerTruckRunningEmissionsV2,
                    PM25BaselineHaulerTruckIdlingEmissions = entity.PM25BaselineHaulerTruckIdlingEmissions,
                    PM25SmashingEmissions = entity.PM25SmashingEmissions,
                    PM25SMTRunningEmissions = entity.PM25SMTRunningEmissions,
                    PM25SMTIdlingEmissions = entity.PM25SMTIdlingEmissions,
                    PM25HaulerRunningEmissionsWithCompactibility = entity.PM25HaulerRunningEmissionsWithCompactibilityV2,
                    PM25HaulerIdlingEmissionsWithCompactibility = entity.PM25HaulerIdlingEmissionsWithCompactibility,*/
                    TotalPM25BaselineTruckEmissionsV2 = entity.TotalPM25BaselineTruckEmissionsV2,
                    TotalPM25EmissionsWithSmashV2 = entity.TotalPM25EmissionsWithSmashV2,
                    PM25PercentSavedV2 = entity.PM25PercentSavedV2,

                    /*PM10BaselineHaulerTruckRunningEmissions = entity.PM10BaselineHaulerTruckRunningEmissionsV2,
                    PM10BaselineHaulerTruckIdlingEmissions = entity.PM10BaselineHaulerTruckIdlingEmissions,
                    PM10SmashingEmissions = entity.PM10SmashingEmissions,
                    PM10SMTRunningEmissions = entity.PM10SMTRunningEmissions,
                    PM10SMTIdlingEmissions = entity.PM10SMTIdlingEmissions,
                    PM10HaulerRunningEmissionsWithCompactibility = entity.PM10HaulerRunningEmissionsWithCompactibilityV2,
                    PM10HaulerIdlingEmissionsWithCompactibility = entity.PM10HaulerIdlingEmissionsWithCompactibility,*/
                    TotalPM10BaselineTruckEmissionsV2 = entity.TotalPM10BaselineTruckEmissionsV2,
                    TotalPM10EmissionsWithSmashV2 = entity.TotalPM10EmissionsWithSmashV2,
                    PM10PercentSavedV2 = entity.PM10PercentSavedV2,

                    /*SO2BaselineHaulerTruckRunningEmissions = entity.SO2BaselineHaulerTruckRunningEmissionsV2,
                    SO2BaselineHaulerTruckIdlingEmissions = entity.SO2BaselineHaulerTruckIdlingEmissions,
                    SO2SmashingEmissions = entity.SO2SmashingEmissions,
                    SO2SMTRunningEmissions = entity.SO2SMTRunningEmissions,
                    SO2SMTIdlingEmissions = entity.SO2SMTIdlingEmissions,
                    SO2HaulerRunningEmissionsWithCompactibility = entity.SO2HaulerRunningEmissionsWithCompactibilityV2,
                    SO2HaulerIdlingEmissionsWithCompactibility = entity.SO2HaulerIdlingEmissionsWithCompactibility,*/
                    TotalSO2BaselineTruckEmissionsV2 = entity.TotalSO2BaselineTruckEmissionsV2,
                    TotalSO2EmissionsWithSmashV2 = entity.TotalSO2EmissionsWithSmashV2,
                    SO2PercentSavedV2 = entity.SO2PercentSavedV2,

                    /*CH4BaselineHaulerTruckRunningEmissions = entity.CH4BaselineHaulerTruckRunningEmissionsV2,
                    CH4BaselineHaulerTruckIdlingEmissions = entity.CH4BaselineHaulerTruckIdlingEmissions,
                    CH4SmashingEmissions = entity.CH4SmashingEmissions,
                    CH4SMTRunningEmissions = entity.CH4SMTRunningEmissions,
                    CH4SMTIdlingEmissions = entity.CH4SMTIdlingEmissions,
                    CH4HaulerRunningEmissionsWithCompactibility = entity.CH4HaulerRunningEmissionsWithCompactibilityV2,
                    CH4HaulerIdlingEmissionsWithCompactibility = entity.CH4HaulerIdlingEmissionsWithCompactibility,*/
                    TotalCH4BaselineTruckEmissionsV2 = entity.TotalCH4BaselineTruckEmissionsV2,
                    TotalCH4EmissionsWithSmashV2 = entity.TotalCH4EmissionsWithSmashV2,
                    CH4PercentSavedV2 = entity.CH4PercentSavedV2,

                    /*COBaselineHaulerTruckRunningEmissions = entity.COBaselineHaulerTruckRunningEmissionsV2,
                    COBaselineHaulerTruckIdlingEmissions = entity.COBaselineHaulerTruckIdlingEmissions,
                    COSmashingEmissions = entity.COSmashingEmissions,
                    COSMTRunningEmissions = entity.COSMTRunningEmissions,
                    COSMTIdlingEmissions = entity.COSMTIdlingEmissions,
                    COHaulerRunningEmissionsWithCompactibility = entity.COHaulerRunningEmissionsWithCompactibilityV2,
                    COHaulerIdlingEmissionsWithCompactibility = entity.COHaulerIdlingEmissionsWithCompactibility,*/
                    TotalCOBaselineTruckEmissionsV3 = entity.TotalCOBaselineTruckEmissionsV3,
                    TotalCOEmissionsWithSmashV3 = entity.TotalCOEmissionsWithSmashV3,
                    COPercentSavedV3 = entity.COPercentSavedV3,

                    /*VOCBaselineHaulerTruckRunningEmissions = entity.VOCBaselineHaulerTruckRunningEmissionsV2,
                    VOCBaselineHaulerTruckIdlingEmissions = entity.VOCBaselineHaulerTruckIdlingEmissions,
                    VOCSmashingEmissions = entity.VOCSmashingEmissions,
                    VOCSMTRunningEmissions = entity.VOCSMTRunningEmissions,
                    VOCSMTIdlingEmissions = entity.VOCSMTIdlingEmissions,
                    VOCHaulerRunningEmissionsWithCompactibility = entity.VOCHaulerRunningEmissionsWithCompactibilityV2,
                    VOCHaulerIdlingEmissionsWithCompactibility = entity.VOCHaulerIdlingEmissionsWithCompactibility,*/
                    TotalVOCBaselineTruckEmissionsV2 = entity.TotalVOCBaselineTruckEmissionsV2,
                    TotalVOCEmissionsWithSmashV2 = entity.TotalVOCEmissionsWithSmashV2,
                    VOCPercentSavedV2 = entity.VOCPercentSavedV2,

                    /*CO2BaselineHaulerTruckRunningEmissions = entity.CO2BaselineHaulerTruckRunningEmissionsV2,
                    CO2BaselineHaulerTruckIdlingEmissions = entity.CO2BaselineHaulerTruckIdlingEmissions,
                    CO2SmashingEmissions = entity.CO2SmashingEmissions,
                    CO2SMTRunningEmissions = entity.CO2SMTRunningEmissions,
                    CO2SMTIdlingEmissions = entity.CO2SMTIdlingEmissions,
                    CO2HaulerRunningEmissionsWithCompactibility = entity.CO2HaulerRunningEmissionsWithCompactibilityV2,
                    CO2HaulerIdlingEmissionsWithCompactibility = entity.CO2HaulerIdlingEmissionsWithCompactibility,*/
                    TotalCO2BaselineTruckEmissionsV2 = entity.TotalCO2BaselineTruckEmissionsV2,
                    TotalCO2EmissionsWithSmashV2 = entity.TotalCO2EmissionsWithSmashV2,
                    CO2PercentSavedV2 = entity.CO2PercentSavedV2,



                };
            }
        }


        public bool UpdateBusinesses(BusinessEdit model)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity =
                    ctx
                        .Clients
                        .SingleOrDefault(e => e.BusinessId == model.BusinessId);

                // If the entity does not exist, return false.
                if (entity == null)
                {
                    return false;
                }

                // Update the entity's properties.
                entity.BusinessName = model.BusinessName;
                entity.FacilityID = model.FacilityID;
                entity.State = model.State;
                entity.City = model.City;
                entity.Address = model.Address;
                entity.ZipCode = model.ZipCode;
                entity.Compactibility = model.Compactibility;
                entity.ToClientDist = model.ToClientDist;
                entity.FromClientDist = model.FromClientDist;
                entity.ToHaulerDist = model.ToHaulerDist;
                entity.FromHaulerDist = model.FromHaulerDist;
                entity.LandfillDist = model.LandfillDist;
                entity.HaulsPerWeek = model.HaulsPerWeek;
                entity.NumberOfDumpsters = model.NumberOfDumpsters;

                // Update the franchise if it has changed.
                if (entity.FranchiseId != model.FranchiseId)
                {
                    entity.FranchiseId = model.FranchiseId;
                }
                // Update the Account if it has changed.
                if (entity.AccountId != model.AccountId)
                {
                    entity.AccountId = model.AccountId;
                }

                // Save the changes to the database.
                ctx.SaveChanges();

                // Return true.
                return true;
            }
        }

        public bool DeleteBusiness(int businessId)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity =
                    ctx
                    .Clients
                    .Single(e => e.BusinessId == businessId);

                ctx.Clients.Remove(entity);

                return ctx.SaveChanges() == 1;
            }
        }

        // Add this method to your ClientService class
        public async Task<bool> SaveClientsAsync(List<Client> clients)
        {
            using (var ctx = new ApplicationDbContext())
            {
                foreach (var client in clients)
                {
                    // Assuming you have a function to create a new Client entity and add it to the database
                    // You may need to modify this line to match your actual implementation
                    ctx.Clients.Add(client);
                }

                // Save the changes
                await ctx.SaveChangesAsync();
            }

            return true;
        }

        public BusinessDetails GetActiveBusinessById(int id)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity =
                    ctx
                        .Clients
                        .Single(e => e.BusinessId == id && e.IsActive); // Only returns if IsActive is true

                if (entity == null)
                    return null;

                return
                    new BusinessDetails
                    {
                        BusinessId = entity.BusinessId,
                        BusinessName = entity.BusinessName,
                        FacilityID = entity.FacilityID,
                        State = entity.State,
                        City = entity.City,
                        Address = entity.Address,
                        ZipCode = entity.ZipCode,
                        //FranchiseeId = entity.FranchiseeId,
                        //FranchiseeName = entity.Franchisee.FranchiseeName,
                        FranchiseId = entity.Franchise.FranchiseId,
                        FranchiseName = entity.Franchise.FranchiseName,
                        Compactibility = entity.Compactibility,
                        ToClientDist = entity.ToClientDist,
                        FromClientDist = entity.FromClientDist,
                        ToHaulerDist = entity.ToHaulerDist,
                        FromHaulerDist = entity.FromHaulerDist,
                        LandfillDist = entity.LandfillDist,
                        HaulsPerWeek = entity.HaulsPerWeek,
                        NumberOfDumpsters = entity.NumberOfDumpsters,
                        PreSMTYearlyHauls = entity.PreSMTYearlyHauls,
                        TotalCO2SavedV2 = entity.TotalCO2SavedV2,

                        AllEmissionsBaselineTotalsV2 = entity.AllEmissionsBaselineTotalsV2,
                        AllEmissionsWithSmashTotalsV2 = entity.AllEmissionsWithSmashTotalsV2,
                        AllEmissionsSavedWithSmashV2 = entity.AllEmissionsSavedWithSmashV2,
                        AllSavingsTotalV2 = entity.AllSavingsTotalV2,

                        /*NOXBaselineHaulerTruckRunningEmissions = entity.NOXBaselineHaulerTruckRunningEmissionsV2,
                        NOXBaselineHaulerTruckIdlingEmissions = entity.NOXBaselineHaulerTruckIdlingEmissions,
                        NOXSmashingEmissions = entity.NOXSmashingEmissions,
                        NOXSMTRunningEmissions = entity.NOXSMTRunningEmissions,
                        NOXSMTIdlingEmissions = entity.NOXSMTIdlingEmissions,
                        NOXHaulerRunningEmissionsWithCompactibility = entity.NOXHaulerRunningEmissionsWithCompactibilityV2,
                        NOXHaulerIdlingEmissionsWithCompactibility = entity.NOXHaulerIdlingEmissionsWithCompactibility,*/
                        TotalNOXBaselineTruckEmissionsV2 = entity.TotalNOXBaselineTruckEmissionsV2,
                        TotalNOXEmissionsWithSmashV2 = entity.TotalNOXEmissionsWithSmashV2,
                        NOXPercentSavedV2 = entity.NOXPercentSavedV2,

                        /*N20BaselineHaulerTruckRunningEmissions = entity.N20BaselineHaulerTruckRunningEmissionsV2,
                        N20BaselineHaulerTruckIdlingEmissions= entity.N20BaselineHaulerTruckIdlingEmissions,
                        N20SmashingEmissions= entity.N20SmashingEmissions,
                        N20SMTRunningEmissions= entity.N20SMTRunningEmissions,
                        N20SMTIdlingEmissions= entity.N20SMTIdlingEmissions,
                        N20HaulerRunningEmissionsWithCompactibility= entity.N20HaulerRunningEmissionsWithCompactibilityV2,
                        N20HaulerIdlingEmissionsWithCompactibility= entity.N20HaulerIdlingEmissionsWithCompactibility,*/
                        TotalN20BaselineTruckEmissionsV2 = entity.TotalN20BaselineTruckEmissionsV2,
                        TotalN20EmissionsWithSmashV2 = entity.TotalN20EmissionsWithSmashV2,
                        N20PercentSavedV2 = entity.N20PercentSavedV2,

                        /*PM25BaselineHaulerTruckRunningEmissions = entity.PM25BaselineHaulerTruckRunningEmissionsV2,
                        PM25BaselineHaulerTruckIdlingEmissions = entity.PM25BaselineHaulerTruckIdlingEmissions,
                        PM25SmashingEmissions = entity.PM25SmashingEmissions,
                        PM25SMTRunningEmissions = entity.PM25SMTRunningEmissions,
                        PM25SMTIdlingEmissions = entity.PM25SMTIdlingEmissions,
                        PM25HaulerRunningEmissionsWithCompactibility = entity.PM25HaulerRunningEmissionsWithCompactibilityV2,
                        PM25HaulerIdlingEmissionsWithCompactibility = entity.PM25HaulerIdlingEmissionsWithCompactibility,*/
                        TotalPM25BaselineTruckEmissionsV2 = entity.TotalPM25BaselineTruckEmissionsV2,
                        TotalPM25EmissionsWithSmashV2 = entity.TotalPM25EmissionsWithSmashV2,
                        PM25PercentSavedV2 = entity.PM25PercentSavedV2,

                        /*PM10BaselineHaulerTruckRunningEmissions = entity.PM10BaselineHaulerTruckRunningEmissionsV2,
                        PM10BaselineHaulerTruckIdlingEmissions = entity.PM10BaselineHaulerTruckIdlingEmissions,
                        PM10SmashingEmissions = entity.PM10SmashingEmissions,
                        PM10SMTRunningEmissions = entity.PM10SMTRunningEmissions,
                        PM10SMTIdlingEmissions = entity.PM10SMTIdlingEmissions,
                        PM10HaulerRunningEmissionsWithCompactibility = entity.PM10HaulerRunningEmissionsWithCompactibilityV2,
                        PM10HaulerIdlingEmissionsWithCompactibility = entity.PM10HaulerIdlingEmissionsWithCompactibility,*/
                        TotalPM10BaselineTruckEmissionsV2 = entity.TotalPM10BaselineTruckEmissionsV2,
                        TotalPM10EmissionsWithSmashV2 = entity.TotalPM10EmissionsWithSmashV2,
                        PM10PercentSavedV2 = entity.PM10PercentSavedV2,

                        /*SO2BaselineHaulerTruckRunningEmissions = entity.SO2BaselineHaulerTruckRunningEmissionsV2,
                        SO2BaselineHaulerTruckIdlingEmissions = entity.SO2BaselineHaulerTruckIdlingEmissions,
                        SO2SmashingEmissions = entity.SO2SmashingEmissions,
                        SO2SMTRunningEmissions = entity.SO2SMTRunningEmissions,
                        SO2SMTIdlingEmissions = entity.SO2SMTIdlingEmissions,
                        SO2HaulerRunningEmissionsWithCompactibility = entity.SO2HaulerRunningEmissionsWithCompactibilityV2,
                        SO2HaulerIdlingEmissionsWithCompactibility = entity.SO2HaulerIdlingEmissionsWithCompactibility,*/
                        TotalSO2BaselineTruckEmissionsV2 = entity.TotalSO2BaselineTruckEmissionsV2,
                        TotalSO2EmissionsWithSmashV2 = entity.TotalSO2EmissionsWithSmashV2,
                        SO2PercentSavedV2 = entity.SO2PercentSavedV2,

                        /*CH4BaselineHaulerTruckRunningEmissions = entity.CH4BaselineHaulerTruckRunningEmissionsV2,
                        CH4BaselineHaulerTruckIdlingEmissions = entity.CH4BaselineHaulerTruckIdlingEmissions,
                        CH4SmashingEmissions = entity.CH4SmashingEmissions,
                        CH4SMTRunningEmissions = entity.CH4SMTRunningEmissions,
                        CH4SMTIdlingEmissions = entity.CH4SMTIdlingEmissions,
                        CH4HaulerRunningEmissionsWithCompactibility = entity.CH4HaulerRunningEmissionsWithCompactibilityV2,
                        CH4HaulerIdlingEmissionsWithCompactibility = entity.CH4HaulerIdlingEmissionsWithCompactibility,*/
                        TotalCH4BaselineTruckEmissionsV2 = entity.TotalCH4BaselineTruckEmissionsV2,
                        TotalCH4EmissionsWithSmashV2 = entity.TotalCH4EmissionsWithSmashV2,
                        CH4PercentSavedV2 = entity.CH4PercentSavedV2,

                        /*COBaselineHaulerTruckRunningEmissions = entity.COBaselineHaulerTruckRunningEmissionsV2,
                        COBaselineHaulerTruckIdlingEmissions = entity.COBaselineHaulerTruckIdlingEmissions,
                        COSmashingEmissions = entity.COSmashingEmissions,
                        COSMTRunningEmissions = entity.COSMTRunningEmissions,
                        COSMTIdlingEmissions = entity.COSMTIdlingEmissions,
                        COHaulerRunningEmissionsWithCompactibility = entity.COHaulerRunningEmissionsWithCompactibilityV2,
                        COHaulerIdlingEmissionsWithCompactibility = entity.COHaulerIdlingEmissionsWithCompactibility,*/
                        TotalCOBaselineTruckEmissionsV3 = entity.TotalCOBaselineTruckEmissionsV3,
                        TotalCOEmissionsWithSmashV3 = entity.TotalCOEmissionsWithSmashV3,
                        COPercentSavedV3 = entity.COPercentSavedV3,

                        /*VOCBaselineHaulerTruckRunningEmissions = entity.VOCBaselineHaulerTruckRunningEmissionsV2,
                        VOCBaselineHaulerTruckIdlingEmissions = entity.VOCBaselineHaulerTruckIdlingEmissions,
                        VOCSmashingEmissions = entity.VOCSmashingEmissions,
                        VOCSMTRunningEmissions = entity.VOCSMTRunningEmissions,
                        VOCSMTIdlingEmissions = entity.VOCSMTIdlingEmissions,
                        VOCHaulerRunningEmissionsWithCompactibility = entity.VOCHaulerRunningEmissionsWithCompactibilityV2,
                        VOCHaulerIdlingEmissionsWithCompactibility = entity.VOCHaulerIdlingEmissionsWithCompactibility,*/
                        TotalVOCBaselineTruckEmissionsV2 = entity.TotalVOCBaselineTruckEmissionsV2,
                        TotalVOCEmissionsWithSmashV2 = entity.TotalVOCEmissionsWithSmashV2,
                        VOCPercentSavedV2 = entity.VOCPercentSavedV2,

                        /*CO2BaselineHaulerTruckRunningEmissions = entity.CO2BaselineHaulerTruckRunningEmissionsV2,
                        CO2BaselineHaulerTruckIdlingEmissions = entity.CO2BaselineHaulerTruckIdlingEmissions,
                        CO2SmashingEmissions = entity.CO2SmashingEmissions,
                        CO2SMTRunningEmissions = entity.CO2SMTRunningEmissions,
                        CO2SMTIdlingEmissions = entity.CO2SMTIdlingEmissions,
                        CO2HaulerRunningEmissionsWithCompactibility = entity.CO2HaulerRunningEmissionsWithCompactibilityV2,
                        CO2HaulerIdlingEmissionsWithCompactibility = entity.CO2HaulerIdlingEmissionsWithCompactibility,*/
                        TotalCO2BaselineTruckEmissionsV2 = entity.TotalCO2BaselineTruckEmissionsV2,
                        TotalCO2EmissionsWithSmashV2 = entity.TotalCO2EmissionsWithSmashV2,
                        CO2PercentSavedV2 = entity.CO2PercentSavedV2,
                    };
            }
        }


        public void DeactivateBusiness(int businessId)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity =
                    ctx
                        .Clients
                        .Single(e => e.BusinessId == businessId);

                entity.IsActive = false;

                ctx.SaveChanges();
            }
        }


        public class ClientImportModel
        {
            [Name("franchise")]
            public string FranchiseName { get; set; }

            [Name("franchiseID")]
            public int FranchiseId { get; set; }

            [Name("clientName")]
            public string ClientName { get; set; }

            [Name("clientID")]
            public int ClientId { get; set; }

            [Name("serviceLocation")]
            public string ServiceLocation { get; set; }

            [Name("streetNo")]
            public string StreetNumber { get; set; }

            [Name("locationStreet")]
            public string Street { get; set; }

            [Name("city")]
            public string City { get; set; }

            [Name("state")]
            public string State { get; set; }

            [Name("zip")]
            public string ZipCode { get; set; }

            [Name("contactName")]
            public string ContactName { get; set; }
        }

        public class ClientImportModelMap : ClassMap<ClientImportModel>
        {
            public ClientImportModelMap()
            {
                Map(m => m.FranchiseName).Name("franchise");
                Map(m => m.FranchiseId).Name("franchiseID");
                Map(m => m.ClientName).Name("clientName");
                Map(m => m.ClientId).Name("clientID");
                Map(m => m.ServiceLocation).Name("serviceLocation");
                Map(m => m.StreetNumber).Name("streetNo");
                Map(m => m.Street).Name("locationStreet");
                Map(m => m.City).Name("city");
                Map(m => m.State).Name("state");
                Map(m => m.ZipCode).Name("zip");
                Map(m => m.ContactName).Name("contactName");
            }
        }


        public void ImportClientsFromCsv(string relativeFilePath)
        {
            string filePath = System.Web.Hosting.HostingEnvironment.MapPath(relativeFilePath);
            int processedCount = 0;

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    using (var reader = new StreamReader(filePath))
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        var records = csv.GetRecords<ClientImportModel>().ToList();

                        foreach (var record in records)
                        {
                            try
                            {
                                int franchiseId = FindFranchiseId(record.FranchiseName, record.FranchiseId);
                                var existingClient = _context.Clients.FirstOrDefault(c => c.BusinessId == record.ClientId);

                                if (existingClient != null)
                                {
                                    UpdateExistingClient(existingClient, record, franchiseId);
                                    System.Diagnostics.Debug.WriteLine($"Updated client: {record.ClientName}");
                                }
                                else
                                {
                                    InsertNewClient(record, franchiseId);
                                    System.Diagnostics.Debug.WriteLine($"Added new client: {record.ClientName}");
                                }

                                processedCount++;
                            }
                            catch (CsvHelper.TypeConversion.TypeConverterException ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"Error in record {record.ClientId}: {ex.Message}");
                                continue;
                            }
                        }

                        _context.SaveChanges();
                    }

                    transaction.Commit();
                    System.Diagnostics.Debug.WriteLine($"Total clients processed: {processedCount}");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    System.Diagnostics.Debug.WriteLine("Import failed: " + ex.Message);
                    throw;
                }
            }
        }


        private void UpdateExistingClient(Client existingClient, ClientImportModel record, int franchiseId)
        {
            existingClient.BusinessName = record.ClientName;
            existingClient.FacilityID = record.ServiceLocation; // Adjust if different
            existingClient.StreetNumber = record.StreetNumber;
            existingClient.Address = record.Street;
            existingClient.City = record.City;
            existingClient.State = ConvertState(record.State); // Convert to Enum
            existingClient.ZipCode = record.ZipCode;
            existingClient.ContactName = record.ContactName;
            existingClient.FranchiseId = franchiseId;
            // Add or modify other fields as needed
        }

        private void InsertNewClient(ClientImportModel record, int franchiseId)
        {
            var newClient = new Client
            {
                BusinessId = record.ClientId,
                BusinessName = record.ClientName,
                FacilityID = record.ServiceLocation, // Adjust if different
                StreetNumber = record.StreetNumber,
                Address = record.Street,
                City = record.City,
                State = ConvertState(record.State), // Convert to Enum
                ZipCode = record.ZipCode,
                ContactName = record.ContactName,
                FranchiseId = franchiseId,
                // Add or modify other fields as needed
            };

            _context.Clients.Add(newClient);
        }
        // Ensure your State conversion method is appropriate for your needs
        private State ConvertState(string stateAbbreviation)
        {
            if (Enum.TryParse<State>(stateAbbreviation, true, out var stateEnum))
            {
                return stateEnum;
            }
            else
            {
                // Handle cases where state abbreviation is invalid
                return State.Unknown; // Or any default/placeholder value you prefer
            }
        }

        private int FindFranchiseId(string franchiseName, int franchiseId)
        {
            // Convert both the names to lower (or upper) case for case-insensitive comparison
            var franchise = _context.Franchises
                .FirstOrDefault(f => f.FranchiseName.Trim().ToLower() == franchiseName.Trim().ToLower());


            if (franchise != null)
            {
                return franchise.FranchiseId;
            }
            else
            {
                // Log the error for debugging
                // For example: Log.Error($"Franchise not found for name: {franchiseName}");
                return -1;
            }
        }

        public bool AssignOrphanClientsToFranchise(int targetFranchiseId)
        {
            // Retrieve all clients that do not have a FranchiseId assigned.
            var orphanClients = _context.Clients.Where(client => client.FranchiseId == null).ToList();

            if (!orphanClients.Any())
            {
                // No orphan clients found.
                return false;
            }

            // Retrieve the target franchise to ensure it exists.
            var franchise = _context.Franchises.Find(targetFranchiseId);
            if (franchise == null)
            {
                // Franchise with the given ID does not exist.
                return false;
            }

            // Assign each orphan client to the target franchise.
            foreach (var client in orphanClients)
            {
                client.FranchiseId = targetFranchiseId;
            }

            // Save the changes to the database.
            return _context.SaveChanges() > 0;
        }


    }
}
