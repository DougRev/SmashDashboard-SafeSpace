namespace BusinesssMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Business",
                c => new
                    {
                        BusinessId = c.Int(nullable: false, identity: true),
                        BusinessName = c.String(),
                        BusinessIdNumber = c.Int(),
                    })
                .PrimaryKey(t => t.BusinessId);
            
            CreateTable(
                "dbo.Franchisee",
                c => new
                    {
                        FranchiseeId = c.Int(nullable: false, identity: true),
                        FranchiseeName = c.String(),
                        Business_BusinessId = c.Int(),
                        BusinesssFranchisee_FranchiseeId = c.Int(),
                        BusinesssFranchisee_BusinessId = c.Int(),
                    })
                .PrimaryKey(t => t.FranchiseeId)
                .ForeignKey("dbo.Business", t => t.Business_BusinessId)
                .ForeignKey("dbo.BusinesssFranchisee", t => new { t.BusinesssFranchisee_FranchiseeId, t.BusinesssFranchisee_BusinessId })
                .Index(t => t.Business_BusinessId)
                .Index(t => new { t.BusinesssFranchisee_FranchiseeId, t.BusinesssFranchisee_BusinessId });
            
            CreateTable(
                "dbo.BusinesssFranchisee",
                c => new
                    {
                        FranchiseeId = c.Int(nullable: false),
                        BusinessId = c.Int(nullable: false),
                        Franchisee_FranchiseeId = c.Int(),
                    })
                .PrimaryKey(t => new { t.FranchiseeId, t.BusinessId })
                .ForeignKey("dbo.Business", t => t.BusinessId, cascadeDelete: true)
                .ForeignKey("dbo.Franchisee", t => t.Franchisee_FranchiseeId)
                .Index(t => t.BusinessId)
                .Index(t => t.Franchisee_FranchiseeId);
            
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
            DropForeignKey("dbo.Franchisee", new[] { "BusinesssFranchisee_FranchiseeId", "BusinesssFranchisee_BusinessId" }, "dbo.BusinesssFranchisee");
            DropForeignKey("dbo.BusinesssFranchisee", "Franchisee_FranchiseeId", "dbo.Franchisee");
            DropForeignKey("dbo.BusinesssFranchisee", "BusinessId", "dbo.Business");
            DropForeignKey("dbo.Franchisee", "Business_BusinessId", "dbo.Business");
            DropIndex("dbo.IdentityUserLogin", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.IdentityUserClaim", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.IdentityUserRole", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.IdentityUserRole", new[] { "IdentityRole_Id" });
            DropIndex("dbo.BusinesssFranchisee", new[] { "Franchisee_FranchiseeId" });
            DropIndex("dbo.BusinesssFranchisee", new[] { "BusinessId" });
            DropIndex("dbo.Franchisee", new[] { "BusinesssFranchisee_FranchiseeId", "BusinesssFranchisee_BusinessId" });
            DropIndex("dbo.Franchisee", new[] { "Business_BusinessId" });
            DropTable("dbo.IdentityUserLogin");
            DropTable("dbo.IdentityUserClaim");
            DropTable("dbo.ApplicationUser");
            DropTable("dbo.IdentityUserRole");
            DropTable("dbo.IdentityRole");
            DropTable("dbo.BusinesssFranchisee");
            DropTable("dbo.Franchisee");
            DropTable("dbo.Business");
        }
    }
}
