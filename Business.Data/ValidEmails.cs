﻿using BusinessData.Enum;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNet.Identity;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto.Macs;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Security.Policy;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace BusinessData
{
    public static class ValidEmails
    {
        public static List<string> EmailList = new List<string>
        {
            "aaron.huntley@smashmytrash.com",
            "adam.wilver@smashmytrash.com",
            "techops@smashmytrash.com",
            "adrienne.capers@smashmytrash.com",
            "alycia.goss@smashmytrash.com",
            "andy.shepherd@smashmytrash.com",
            "ashley.petty@smashmytrash.com",
            "bill.morris@smashmytrash.com",
            "bphillips@globalresponse.com",
            "brad.gillespie@smashmytrash.com",
            "brandi.perossa@smashmytrash.com",
            "bret.borock@smashmytrash.com",
            "brian.reeve@smashmytrash.com",
            "bridget.goffiney@smashmytrash.com",
            "cameron.normoyle@smashmytrash.com",
            "carecenter@cits.canon.com",
            "carly.hellickson@smashmytrash.com",
            "chris.bone@smashmytrash.com",
            "chris.martin@smashmytrash.com",
            "chris.price@smashmytrash.com",
            "christian.valiulis@smashmytrash.com",
            "christopher.schenk@smashmytrash.com",
            "chuck.adams@smashmytrash.com",
            "chuck.sullivan@smashmytrash.com",
            "cole.dunaway@smashmytrash.com",
            "comito.stephen@sbcglobal.net",
            "connor.groce@smashmytrash.com",
            "craig.czubik@smashmytrash.com",
            "dan.auker@smashmytrash.com",
            "daniel.mcgill@smashmytrash.com",
            "daphne.lippott@smashmytrash.com",
            "dave.price@smashmytrash.com",
            "david.orr@smashmytrash.com",
            "derek.lehman@smashmytrash.com",
            "derek.metz@smashmytrash.com",
            "doug.revelljr@gmail.com",
            "doug.revell@smashmytrash.com",
            "eric.brandt@smashmytrash.com",
            "eric.capers@smashmytrash.com",
            "eric.kissell@smashmytrash.com",
            "evan.reller@smashmytrash.com",
            "franchisechanges@smashmytrash.com",
            "gordon.housman@smashmytrash.com",
            "greg.post@smashmytrash.com",
            "greg.solorio@smashmytrash.com",
            "gregory.rogers@smashmytrash.com",
            "heath.reid@smashmytrash.com",
            "jasmine.elliott@smashmytrash.com",
            "jason.lefkowitz@smashmytrash.com",
            "jeanette.metz@smashmytrash.com",
            "jeff.deetz@smashmytrash.com",
            "jeff.fehr@smashmytrash.com",
            "jeff.huggins@smashmytrash.com",
            "jeff.phillips@smashmytrash.com",
            "jeff.stockton@smashmytrash.com",
            "jeremy.chase@smashmytrash.com",
            "jeremy.dunlap@smashmytrash.com",
            "jim.st.louis@smashmytrash.com",
            "joe.oley@smashmytrash.com",
            "John.Coulter@fivestarcallcenters.com",
            "john.hellickson@smashmytrash.com",
            "john.lombardo@smashmytrash.com",
            "john.ramsier@smashmytrash.com",
            "john.silcox@smashmytrash.com",
            "jonathan.phelps@smashmytrash.com",
            "joseph.russo@smashmytrash.com",
            "joy.liggon@peaksupport.io",
            "kal.patel@smashmytrash.com",
            "kara.garcia@smashmytrash.com",
            "karen.seeley@smashmytrash.com",
            "karen.zwink@smashmytrash.com",
            "keegan.orellana@smashmytrash.com",
            "keith.leimbach@smashmytrash.com",
            "kevin.brown@smashmytrash.com",
            "kevin.moyer@smashmytrash.com",
            "kimberly.price@smashmytrash.com",
            "kirk.anderson@smashmytrash.com",
            "kyle.cramer@smashmytrash.com",
            "kyle.granger@smashmytrash.com",
            "klarson@365roi.com",
            "lisa.kissell@smashmytrash.com",
            "makaylynn.eiler@smashmytrash.com",
            "marc.fraga@smashmytrash.com",
            "maria.meeuwisse@icloud.com",
            "marketing@smashmytrash.com",
            "matt.beatty@smashmytrash.com",
            "matt.grabowski@smashmytrash.com",
            "michael.knapick@smashmytrash.com",
            "michael.munnerlyn@smashmytrash.com",
            "michele.brizzi@smashmytrash.com",
            "michelle.taylor@smashmytrash.com",
            "mike.berryman@smashmytrash.com",
            "mike.hagan@smashmytrash.com",
            "nathan.pettersen@smashmytrash.com",
            "nicole.rodden@smashmytrash.com",
            "oscar.villanueva@smashmytrash.com",
            "patrick.kardasz@smashmytrash.com",
            "paul.bell@smashmytrash.com",
            "paul.carnot@smashmytrash.com",
            "paul.nejezchleb@smashmytrash.com",
            "philip.shields@smashmytrash.com",
            "phishing@smashmytrash.com",
            "rene.solorio@smashmytrash.com",
            "rich.spitzer@smashmytrash.com",
            "robert.kite@smashmytrash.com",
            "robert.rodden@smashmytrash.com",
            "robert.schoenfeld@smashmytrash.com",
            "robert.seeley@smashmytrash.com",
            "russell.steger@smashmytrash.com",
            "sam.perossa@smashmytrash.com",
            "sarb.singh@smashmytrash.com",
            "shelly.valiulis@smashmytrash.com",
            "sidney.rutowski@smashmytrash.com",
            "smt-service-account@smash-dashboard.iam.gserviceaccount.com",
            "steve@apexcallcenters.com",
            "steve.radcliff@smashmytrash.com",
            "steve.shiffman@smashmytrash.com",
            "tammy.wood@smashmytrash.com",
            "Teresa.Ayers@atg.in.gov",
            "tim.hazzard@smashmytrash.com",
            "tim.hunt@smashmytrash.com",
            "tim.rutowski@smashmytrash.com",
            "todd.wilson@smashmytrash.com",
            "zach.beam@smashmytrash.com"
        };

        public static List<string> NationalAccountsEmailList = new List<string>
        {
            // email addresses that have access to the National Accounts page
            "chris.bone@smashmytrash.com",
            "craig.czubik@smashmytrash.com",
            "chuck.adams@smashmytrash.com",
            "doug.revell@smashmytrash.com",
        };
    }
}
