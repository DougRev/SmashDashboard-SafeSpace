namespace BusinessData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeContactPhonetoString : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Invoice", "ContactPhone", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Invoice", "ContactPhone", c => c.Int(nullable: false));
        }
    }
}
