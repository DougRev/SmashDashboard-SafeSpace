namespace BusinessData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BusinesssFranchisee",
                c => new
                    {
                        FranchiseeId = c.Int(nullable: false),
                        BusinessId = c.Int(nullable: false),
                        Franchisee_FranchiseeId = c.Int(),
                    })
                .PrimaryKey(t => new { t.FranchiseeId, t.BusinessId })
                .ForeignKey("dbo.Client", t => t.BusinessId, cascadeDelete: true)
                .ForeignKey("dbo.FranchiseOwner", t => t.Franchisee_FranchiseeId)
                .Index(t => t.BusinessId)
                .Index(t => t.Franchisee_FranchiseeId);
            
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
                .ForeignKey("dbo.Franchise", t => t.FranchiseId, cascadeDelete: true)
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
                        LaunchDate = c.String(),
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
                "dbo.FranchiseOwner",
                c => new
                    {
                        FranchiseeId = c.Int(nullable: false, identity: true),
                        OwnerId = c.Guid(nullable: false),
                        FranchiseeName = c.String(),
                        FranchiseId = c.Int(nullable: false),
                        BusinesssFranchisee_FranchiseeId = c.Int(),
                        BusinesssFranchisee_BusinessId = c.Int(),
                    })
                .PrimaryKey(t => t.FranchiseeId)
                .ForeignKey("dbo.Franchise", t => t.FranchiseId)
                .ForeignKey("dbo.BusinesssFranchisee", t => new { t.BusinesssFranchisee_FranchiseeId, t.BusinesssFranchisee_BusinessId })
                .Index(t => t.FranchiseId)
                .Index(t => new { t.BusinesssFranchisee_FranchiseeId, t.BusinesssFranchisee_BusinessId });
            
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
                "dbo.IdentityRole",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.IdentityUserRole",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(),
                        IdentityRole_Id = c.String(maxLength: 128),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.IdentityRole", t => t.IdentityRole_Id)
                .ForeignKey("dbo.ApplicationUser", t => t.ApplicationUser_Id)
                .Index(t => t.IdentityRole_Id)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.ApplicationUser",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.IdentityUserClaim",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ApplicationUser", t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.IdentityUserLogin",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        LoginProvider = c.String(),
                        ProviderKey = c.String(),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.ApplicationUser", t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.IdentityUserRole", "ApplicationUser_Id", "dbo.ApplicationUser");
            DropForeignKey("dbo.IdentityUserLogin", "ApplicationUser_Id", "dbo.ApplicationUser");
            DropForeignKey("dbo.IdentityUserClaim", "ApplicationUser_Id", "dbo.ApplicationUser");
            DropForeignKey("dbo.IdentityUserRole", "IdentityRole_Id", "dbo.IdentityRole");
            DropForeignKey("dbo.FranchiseOwner", new[] { "BusinesssFranchisee_FranchiseeId", "BusinesssFranchisee_BusinessId" }, "dbo.BusinesssFranchisee");
            DropForeignKey("dbo.BusinesssFranchisee", "Franchisee_FranchiseeId", "dbo.FranchiseOwner");
            DropForeignKey("dbo.BusinesssFranchisee", "BusinessId", "dbo.Client");
            DropForeignKey("dbo.Client", "AccountId", "dbo.NationalAccount");
            DropForeignKey("dbo.FranchiseOwner", "FranchiseId", "dbo.Franchise");
            DropForeignKey("dbo.Client", "FranchiseId", "dbo.Franchise");
            DropIndex("dbo.IdentityUserLogin", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.IdentityUserClaim", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.IdentityUserRole", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.IdentityUserRole", new[] { "IdentityRole_Id" });
            DropIndex("dbo.FranchiseOwner", new[] { "BusinesssFranchisee_FranchiseeId", "BusinesssFranchisee_BusinessId" });
            DropIndex("dbo.FranchiseOwner", new[] { "FranchiseId" });
            DropIndex("dbo.Client", new[] { "AccountId" });
            DropIndex("dbo.Client", new[] { "FranchiseId" });
            DropIndex("dbo.BusinesssFranchisee", new[] { "Franchisee_FranchiseeId" });
            DropIndex("dbo.BusinesssFranchisee", new[] { "BusinessId" });
            DropTable("dbo.IdentityUserLogin");
            DropTable("dbo.IdentityUserClaim");
            DropTable("dbo.ApplicationUser");
            DropTable("dbo.IdentityUserRole");
            DropTable("dbo.IdentityRole");
            DropTable("dbo.NationalAccount");
            DropTable("dbo.FranchiseOwner");
            DropTable("dbo.Franchise");
            DropTable("dbo.Client");
            DropTable("dbo.BusinesssFranchisee");
        }
    }
}
