namespace BusinessData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class makeVinvoiceIdaString : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.WOCharges", "VonigoInvoiceId", c => c.String());
            AlterColumn("dbo.Invoice", "VonigoInvoiceId", c => c.String());
            AlterColumn("dbo.WorkOrder", "VonigoInvoiceId", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.WorkOrder", "VonigoInvoiceId", c => c.Int());
            AlterColumn("dbo.Invoice", "VonigoInvoiceId", c => c.Int(nullable: false));
            AlterColumn("dbo.WOCharges", "VonigoInvoiceId", c => c.Int());
        }
    }
}
