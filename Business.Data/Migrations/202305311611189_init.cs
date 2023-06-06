namespace BusinessData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WOCharges",
                c => new
                    {
                        ChargeId = c.Int(nullable: false, identity: true),
                        ChargeName = c.String(),
                        WorkOrderId = c.Int(nullable: false),
                        Description = c.String(),
                        Charge = c.Single(nullable: false),
                        SubTotal = c.Single(nullable: false),
                        Tax = c.Single(nullable: false),
                        Invoice_InvoiceId = c.Int(),
                    })
                .PrimaryKey(t => t.ChargeId)
                .ForeignKey("dbo.Invoice", t => t.Invoice_InvoiceId)
                .ForeignKey("dbo.WorkOrder", t => t.WorkOrderId, cascadeDelete: true)
                .Index(t => t.WorkOrderId)
                .Index(t => t.Invoice_InvoiceId);
            
            CreateTable(
                "dbo.WorkOrder",
                c => new
                    {
                        WorkOrderId = c.Int(nullable: false, identity: true),
                        InvoiceId = c.Int(nullable: false),
                        FranchiseId = c.Int(nullable: false),
                        CompletedDateTime = c.DateTime(nullable: false),
                        StreetNumber = c.Int(nullable: false),
                        StreetAddress = c.String(),
                        City = c.String(),
                        State = c.String(),
                        Zipcode = c.String(),
                        ContactId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.WorkOrderId)
                .ForeignKey("dbo.Invoice", t => t.InvoiceId, cascadeDelete: true)
                .ForeignKey("dbo.Contact", t => t.ContactId, cascadeDelete: true)
                .ForeignKey("dbo.Franchise", t => t.FranchiseId, cascadeDelete: true)
                .Index(t => t.InvoiceId)
                .Index(t => t.FranchiseId)
                .Index(t => t.ContactId);
            
            CreateTable(
                "dbo.Contact",
                c => new
                    {
                        ContactId = c.Int(nullable: false, identity: true),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Email = c.String(),
                        Phone = c.String(),
                        AccountType = c.String(),
                        ClientId = c.Int(nullable: false),
                        LocationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ContactId)
                .ForeignKey("dbo.Client", t => t.ClientId, cascadeDelete: true)
                .ForeignKey("dbo.Location", t => t.LocationId)
                .Index(t => t.ClientId)
                .Index(t => t.LocationId);
            
            CreateTable(
                "dbo.Client",
                c => new
                    {
                        BusinessId = c.Int(nullable: false, identity: true),
                        IsActive = c.Boolean(nullable: false),
                        BusinessName = c.String(),
                        State = c.Int(nullable: false),
                        FacilityID = c.String(),
                        City = c.String(),
                        Address = c.String(),
                        ZipCode = c.Int(nullable: false),
                        OwnerId = c.Guid(nullable: false),
                        FranchiseId = c.Int(nullable: false),
                        AccountId = c.Int(),
                        AccountName = c.String(),
                        FranchiseName = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        PhoneNumber = c.Int(nullable: false),
                        Email = c.String(),
                        AddToDb = c.Boolean(nullable: false),
                        ToClientDist = c.Single(nullable: false),
                        FromClientDist = c.Single(nullable: false),
                        ToHaulerDist = c.Single(nullable: false),
                        LandfillDist = c.Single(nullable: false),
                        FromHaulerDist = c.Single(nullable: false),
                        HaulsPerDay = c.Int(nullable: false),
                        NumberOfDumpsters = c.Int(nullable: false),
                        Compactibility = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.BusinessId)
                .ForeignKey("dbo.Franchise", t => t.FranchiseId)
                .ForeignKey("dbo.NationalAccount", t => t.AccountId)
                .Index(t => t.FranchiseId)
                .Index(t => t.AccountId);
            
            CreateTable(
                "dbo.Franchise",
                c => new
                    {
                        FranchiseId = c.Int(nullable: false, identity: true),
                        IsActive = c.Boolean(nullable: false),
                        OwnerId = c.Guid(nullable: false),
                        FranchiseName = c.String(),
                        AccountId = c.Int(),
                        State = c.Int(nullable: false),
                        Region = c.String(),
                        LaunchDate = c.DateTime(nullable: false),
                        BusinessAddress = c.String(),
                        BusinessCity = c.String(),
                        BusinessState = c.String(),
                        BusinessZipCode = c.String(),
                        BusinessPhone = c.String(),
                        Status = c.String(),
                        Territories = c.String(),
                        Owner1 = c.String(),
                        Owner1Email = c.String(),
                        Owner1Phone = c.String(),
                        Owner2 = c.String(),
                        Owner2Email = c.String(),
                        Owner2Phone = c.String(),
                        Owner3 = c.String(),
                        Owner3Email = c.String(),
                        Owner3Phone = c.String(),
                        Owner4 = c.String(),
                        Owner4Email = c.String(),
                        Owner4Phone = c.String(),
                    })
                .PrimaryKey(t => t.FranchiseId);
            
            CreateTable(
                "dbo.Invoice",
                c => new
                    {
                        InvoiceId = c.Int(nullable: false, identity: true),
                        FranchiseId = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                        AccountType = c.String(),
                        TotalCost = c.Single(nullable: false),
                        Address = c.String(),
                        Status = c.String(),
                    })
                .PrimaryKey(t => t.InvoiceId)
                .ForeignKey("dbo.Client", t => t.ClientId)
                .ForeignKey("dbo.Franchise", t => t.FranchiseId)
                .Index(t => t.FranchiseId)
                .Index(t => t.ClientId);
            
            CreateTable(
                "dbo.Location",
                c => new
                    {
                        LocationId = c.Int(nullable: false, identity: true),
                        StreetNumber = c.String(),
                        Street = c.String(),
                        City = c.String(),
                        Providence = c.String(),
                        ZipCode = c.Int(nullable: false),
                        AccountType = c.String(),
                        ClientId = c.Int(nullable: false),
                        ContactId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.LocationId)
                .ForeignKey("dbo.Contact", t => t.ContactId, cascadeDelete: true)
                .ForeignKey("dbo.Client", t => t.ClientId)
                .Index(t => t.ClientId)
                .Index(t => t.ContactId);
            
            CreateTable(
                "dbo.NationalAccount",
                c => new
                    {
                        AccountId = c.Int(nullable: false, identity: true),
                        OwnerId = c.Guid(nullable: false),
                        AccountName = c.String(),
                        State = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AccountId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.WOCharges", "WorkOrderId", "dbo.WorkOrder");
            DropForeignKey("dbo.WorkOrder", "FranchiseId", "dbo.Franchise");
            DropForeignKey("dbo.WorkOrder", "ContactId", "dbo.Contact");
            DropForeignKey("dbo.Contact", "LocationId", "dbo.Location");
            DropForeignKey("dbo.Client", "AccountId", "dbo.NationalAccount");
            DropForeignKey("dbo.Location", "ClientId", "dbo.Client");
            DropForeignKey("dbo.Location", "ContactId", "dbo.Contact");
            DropForeignKey("dbo.WorkOrder", "InvoiceId", "dbo.Invoice");
            DropForeignKey("dbo.Invoice", "FranchiseId", "dbo.Franchise");
            DropForeignKey("dbo.Invoice", "ClientId", "dbo.Client");
            DropForeignKey("dbo.WOCharges", "Invoice_InvoiceId", "dbo.Invoice");
            DropForeignKey("dbo.Client", "FranchiseId", "dbo.Franchise");
            DropForeignKey("dbo.Contact", "ClientId", "dbo.Client");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Location", new[] { "ContactId" });
            DropIndex("dbo.Location", new[] { "ClientId" });
            DropIndex("dbo.Invoice", new[] { "ClientId" });
            DropIndex("dbo.Invoice", new[] { "FranchiseId" });
            DropIndex("dbo.Client", new[] { "AccountId" });
            DropIndex("dbo.Client", new[] { "FranchiseId" });
            DropIndex("dbo.Contact", new[] { "LocationId" });
            DropIndex("dbo.Contact", new[] { "ClientId" });
            DropIndex("dbo.WorkOrder", new[] { "ContactId" });
            DropIndex("dbo.WorkOrder", new[] { "FranchiseId" });
            DropIndex("dbo.WorkOrder", new[] { "InvoiceId" });
            DropIndex("dbo.WOCharges", new[] { "Invoice_InvoiceId" });
            DropIndex("dbo.WOCharges", new[] { "WorkOrderId" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.NationalAccount");
            DropTable("dbo.Location");
            DropTable("dbo.Invoice");
            DropTable("dbo.Franchise");
            DropTable("dbo.Client");
            DropTable("dbo.Contact");
            DropTable("dbo.WorkOrder");
            DropTable("dbo.WOCharges");
        }
    }
}
