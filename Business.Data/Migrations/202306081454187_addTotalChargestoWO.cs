namespace BusinessData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTotalChargestoWO : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WorkOrder", "TotalCharges", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.WorkOrder", "TotalCharges");
        }
    }
}
