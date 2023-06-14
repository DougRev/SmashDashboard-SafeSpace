namespace BusinessData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateInvoicewithContactEmailNumber : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Invoice", "SmtContact", c => c.String());
            AddColumn("dbo.Invoice", "AccountContact", c => c.String());
            AddColumn("dbo.Invoice", "ContactEmail", c => c.String());
            AddColumn("dbo.Invoice", "ContactPhone", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Invoice", "ContactPhone");
            DropColumn("dbo.Invoice", "ContactEmail");
            DropColumn("dbo.Invoice", "AccountContact");
            DropColumn("dbo.Invoice", "SmtContact");
        }
    }
}
