namespace BusinessData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class editCharges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WOCharges", "VonigoWorkOrderId", c => c.Int(nullable: false));
            AddColumn("dbo.WOCharges", "VonigoWorkOrderName", c => c.String());
            AddColumn("dbo.WOCharges", "VonigoPriceListName", c => c.String());
            AddColumn("dbo.WOCharges", "FranchiseName", c => c.String());
            AlterColumn("dbo.WOCharges", "Charge", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.WOCharges", "Charge", c => c.Single(nullable: false));
            DropColumn("dbo.WOCharges", "FranchiseName");
            DropColumn("dbo.WOCharges", "VonigoPriceListName");
            DropColumn("dbo.WOCharges", "VonigoWorkOrderName");
            DropColumn("dbo.WOCharges", "VonigoWorkOrderId");
        }
    }
}
