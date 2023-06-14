namespace BusinessData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixingChargeTotalnotReturning : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WOCharges", "Total", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.WOCharges", "Total");
        }
    }
}
