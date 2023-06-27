namespace BusinessData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addInvoiceType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Invoice", "InvoiceType", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Invoice", "InvoiceType");
        }
    }
}
