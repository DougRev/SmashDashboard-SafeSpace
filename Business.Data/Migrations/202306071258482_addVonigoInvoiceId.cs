namespace BusinessData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addVonigoInvoiceId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Invoice", "VonigoInvoiceId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Invoice", "VonigoInvoiceId");
        }
    }
}
