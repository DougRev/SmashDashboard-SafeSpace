namespace BusinessData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WOCharges",
                c => new
                    {
                        ChargeId = c.Int(nullable: false, identity: true),
                        VonigoChargeId = c.Int(nullable: false),
                        ItemType = c.String(),
                        VonigoInvoiceId = c.String(),
                        ChargeName = c.String(),
                        Title = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        EditedDate = c.DateTime(nullable: false),
                        ActiveCan = c.Boolean(nullable: false),
                        SubTotal = c.Single(nullable: false),
                        Tax = c.Single(nullable: false),
                        Quantity = c.Int(nullable: false),
                        Description = c.String(),
                        Charge = c.String(),
                        VonigoWorkOrderId = c.Int(nullable: false),
                        VonigoWorkOrderName = c.String(),
                        VonigoPriceListName = c.String(),
                        VonigoFranchiseId = c.Int(nullable: false),
                        FranchiseName = c.String(),
                        FranchiseId = c.Int(nullable: false),
                        WorkOrderId = c.Int(nullable: false),
                        Total = c.Single(nullable: false),
                        Invoice_InvoiceId = c.Int(),
                    })
                .PrimaryKey(t => t.ChargeId)
                .ForeignKey("dbo.Invoice", t => t.Invoice_InvoiceId)
                .ForeignKey("dbo.Franchise", t => t.FranchiseId)
                .ForeignKey("dbo.WorkOrder", t => t.WorkOrderId, cascadeDelete: true)
                .Index(t => t.FranchiseId)
                .Index(t => t.WorkOrderId)
                .Index(t => t.Invoice_InvoiceId);
            
            CreateTable(
                "dbo.Franchise",
                c => new
                    {
                        FranchiseId = c.Int(nullable: false, identity: true),
                        VonigoFranchiseId = c.Int(nullable: false),
                        VonigoClientId = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        OwnerId = c.Guid(nullable: false),
                        FranchiseName = c.String(),
                        AccountId = c.Int(),
                        State = c.Int(nullable: false),
                        Region = c.String(),
                        LaunchDate = c.DateTime(),
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
                "dbo.Client",
                c => new
                    {
                        BusinessId = c.Int(nullable: false, identity: true),
                        IsActive = c.Boolean(nullable: false),
                        BusinessName = c.String(),
                        FacilityID = c.String(),
                        StreetNumber = c.Int(),
                        Address = c.String(),
                        City = c.String(),
                        State = c.Int(nullable: false),
                        ZipCode = c.String(),
                        ServiceLocation = c.String(),
                        OwnerId = c.Guid(nullable: false),
                        VonigoFranchiseId = c.Int(nullable: false),
                        VonigoClientId = c.Int(nullable: false),
                        FranchiseId = c.Int(nullable: false),
                        AccountId = c.Int(),
                        AccountName = c.String(),
                        FranchiseName = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        ContactName = c.String(),
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
                .ForeignKey("dbo.NationalAccount", t => t.AccountId)
                .ForeignKey("dbo.Franchise", t => t.FranchiseId)
                .Index(t => t.FranchiseId)
                .Index(t => t.AccountId);
            
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
                "dbo.Location",
                c => new
                    {
                        LocationId = c.Int(nullable: false, identity: true),
                        Street = c.String(),
                        City = c.String(),
                        Providence = c.String(),
                        ZipCode = c.String(),
                        AccountType = c.String(),
                        ClientId = c.Int(nullable: false),
                        ContactId = c.Int(),
                    })
                .PrimaryKey(t => t.LocationId)
                .ForeignKey("dbo.Contact", t => t.ContactId)
                .ForeignKey("dbo.Client", t => t.ClientId)
                .Index(t => t.ClientId)
                .Index(t => t.ContactId);
            
            CreateTable(
                "dbo.Invoice",
                c => new
                    {
                        InvoiceId = c.Int(nullable: false, identity: true),
                        FranchiseId = c.Int(nullable: false),
                        VonigoFranchiseId = c.Int(nullable: false),
                        VonigoClientId = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                        AccountType = c.String(),
                        TotalCost = c.Single(nullable: false),
                        Address = c.String(),
                        InvoiceType = c.String(),
                        Status = c.String(),
                        SmtContact = c.String(),
                        AccountContact = c.String(),
                        ContactEmail = c.String(),
                        ContactPhone = c.String(),
                        VonigoInvoiceId = c.String(),
                    })
                .PrimaryKey(t => t.InvoiceId)
                .ForeignKey("dbo.Client", t => t.ClientId)
                .ForeignKey("dbo.Franchise", t => t.FranchiseId)
                .Index(t => t.FranchiseId)
                .Index(t => t.ClientId);
            
            CreateTable(
                "dbo.WorkOrder",
                c => new
                    {
                        WorkOrderId = c.Int(nullable: false, identity: true),
                        VonigoWorkOrderId = c.Int(nullable: false),
                        VonigoClientId = c.Int(nullable: false),
                        Title = c.String(),
                        FranchiseId = c.Int(nullable: false),
                        InvoiceId = c.Int(),
                        CompletedDate = c.DateTime(nullable: false),
                        CompletedTime = c.DateTime(nullable: false),
                        VonigoIsActive = c.Boolean(nullable: false),
                        StreetNumber = c.Int(nullable: false),
                        StreetAddress = c.String(),
                        City = c.String(),
                        State = c.String(),
                        Zipcode = c.String(),
                        LocationId = c.Int(),
                        ContactId = c.Int(),
                        DumpstersSmashed = c.Int(nullable: false),
                        Summary = c.String(),
                        OnSiteContact = c.String(),
                        ServiceType = c.String(),
                        VonigoInvoiceId = c.String(),
                        BeforeFullness = c.Int(nullable: false),
                        AfterFullness = c.Int(nullable: false),
                        TotalCharges = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.WorkOrderId)
                .ForeignKey("dbo.Contact", t => t.ContactId)
                .ForeignKey("dbo.Franchise", t => t.FranchiseId)
                .ForeignKey("dbo.Invoice", t => t.InvoiceId)
                .ForeignKey("dbo.Location", t => t.LocationId)
                .Index(t => t.FranchiseId)
                .Index(t => t.InvoiceId)
                .Index(t => t.LocationId)
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
            DropForeignKey("dbo.WOCharges", "FranchiseId", "dbo.Franchise");
            DropForeignKey("dbo.Client", "FranchiseId", "dbo.Franchise");
            DropForeignKey("dbo.Client", "AccountId", "dbo.NationalAccount");
            DropForeignKey("dbo.Location", "ClientId", "dbo.Client");
            DropForeignKey("dbo.WorkOrder", "LocationId", "dbo.Location");
            DropForeignKey("dbo.WorkOrder", "InvoiceId", "dbo.Invoice");
            DropForeignKey("dbo.WorkOrder", "FranchiseId", "dbo.Franchise");
            DropForeignKey("dbo.WorkOrder", "ContactId", "dbo.Contact");
            DropForeignKey("dbo.Invoice", "FranchiseId", "dbo.Franchise");
            DropForeignKey("dbo.Invoice", "ClientId", "dbo.Client");
            DropForeignKey("dbo.WOCharges", "Invoice_InvoiceId", "dbo.Invoice");
            DropForeignKey("dbo.Contact", "LocationId", "dbo.Location");
            DropForeignKey("dbo.Location", "ContactId", "dbo.Contact");
            DropForeignKey("dbo.Contact", "ClientId", "dbo.Client");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.WorkOrder", new[] { "ContactId" });
            DropIndex("dbo.WorkOrder", new[] { "LocationId" });
            DropIndex("dbo.WorkOrder", new[] { "InvoiceId" });
            DropIndex("dbo.WorkOrder", new[] { "FranchiseId" });
            DropIndex("dbo.Invoice", new[] { "ClientId" });
            DropIndex("dbo.Invoice", new[] { "FranchiseId" });
            DropIndex("dbo.Location", new[] { "ContactId" });
            DropIndex("dbo.Location", new[] { "ClientId" });
            DropIndex("dbo.Contact", new[] { "LocationId" });
            DropIndex("dbo.Contact", new[] { "ClientId" });
            DropIndex("dbo.Client", new[] { "AccountId" });
            DropIndex("dbo.Client", new[] { "FranchiseId" });
            DropIndex("dbo.WOCharges", new[] { "Invoice_InvoiceId" });
            DropIndex("dbo.WOCharges", new[] { "WorkOrderId" });
            DropIndex("dbo.WOCharges", new[] { "FranchiseId" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.NationalAccount");
            DropTable("dbo.WorkOrder");
            DropTable("dbo.Invoice");
            DropTable("dbo.Location");
            DropTable("dbo.Contact");
            DropTable("dbo.Client");
            DropTable("dbo.Franchise");
            DropTable("dbo.WOCharges");
        }
    }
}
