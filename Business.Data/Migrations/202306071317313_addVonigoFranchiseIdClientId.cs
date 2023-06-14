namespace BusinessData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addVonigoFranchiseIdClientId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Franchise", "VonigoFranchiseId", c => c.Int(nullable: false));
            AddColumn("dbo.Franchise", "VonigoClientId", c => c.Int(nullable: false));
            AddColumn("dbo.Invoice", "VonigoFranchiseId", c => c.Int(nullable: false));
            AddColumn("dbo.Invoice", "VonigoClientId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Invoice", "VonigoClientId");
            DropColumn("dbo.Invoice", "VonigoFranchiseId");
            DropColumn("dbo.Franchise", "VonigoClientId");
            DropColumn("dbo.Franchise", "VonigoFranchiseId");
        }
    }
}
