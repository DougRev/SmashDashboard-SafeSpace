namespace BusinessData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addVonigoDataPointsToClient : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Client", "ServiceLocation", c => c.String());
            AddColumn("dbo.Client", "VonigoFranchiseId", c => c.Int(nullable: false));
            AddColumn("dbo.Client", "VonigoClientId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Client", "VonigoClientId");
            DropColumn("dbo.Client", "VonigoFranchiseId");
            DropColumn("dbo.Client", "ServiceLocation");
        }
    }
}
