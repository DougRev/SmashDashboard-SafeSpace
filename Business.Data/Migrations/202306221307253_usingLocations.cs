namespace BusinessData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class usingLocations : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Client", "StreetNumber", c => c.String());
            AddColumn("dbo.WorkOrder", "LocationId", c => c.Int());
            AlterColumn("dbo.WorkOrder", "VonigoInvoiceId", c => c.Int());
            CreateIndex("dbo.WorkOrder", "LocationId");
            AddForeignKey("dbo.WorkOrder", "LocationId", "dbo.Location", "LocationId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WorkOrder", "LocationId", "dbo.Location");
            DropIndex("dbo.WorkOrder", new[] { "LocationId" });
            AlterColumn("dbo.WorkOrder", "VonigoInvoiceId", c => c.Int(nullable: false));
            DropColumn("dbo.WorkOrder", "LocationId");
            DropColumn("dbo.Client", "StreetNumber");
        }
    }
}
