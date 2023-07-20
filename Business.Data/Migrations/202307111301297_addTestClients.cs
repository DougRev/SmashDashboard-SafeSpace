namespace BusinessData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTestClients : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TestClient",
                c => new
                    {
                        BusinessId = c.Int(nullable: false, identity: true),
                        BusinessName = c.String(),
                        StreetNumber = c.Int(),
                        Street = c.String(),
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
                    })
                .PrimaryKey(t => t.BusinessId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TestClient");
        }
    }
}
