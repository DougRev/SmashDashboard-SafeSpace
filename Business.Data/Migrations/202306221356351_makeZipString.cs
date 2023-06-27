namespace BusinessData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class makeZipString : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.WOCharges", "VonigoInvoiceId", c => c.Int());
            AlterColumn("dbo.Client", "ZipCode", c => c.String());
            AlterColumn("dbo.Location", "ZipCode", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Location", "ZipCode", c => c.Int(nullable: false));
            AlterColumn("dbo.Client", "ZipCode", c => c.Int(nullable: false));
            AlterColumn("dbo.WOCharges", "VonigoInvoiceId", c => c.Int(nullable: false));
        }
    }
}
