namespace BusinessData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeIdentityModelsforWO : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.WorkOrder", new[] { "InvoiceId" });
            AlterColumn("dbo.WorkOrder", "InvoiceId", c => c.Int());
            CreateIndex("dbo.WorkOrder", "InvoiceId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.WorkOrder", new[] { "InvoiceId" });
            AlterColumn("dbo.WorkOrder", "InvoiceId", c => c.Int(nullable: false));
            CreateIndex("dbo.WorkOrder", "InvoiceId");
        }
    }
}
