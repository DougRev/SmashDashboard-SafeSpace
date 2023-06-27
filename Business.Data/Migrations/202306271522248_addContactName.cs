namespace BusinessData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addContactName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Client", "ContactName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Client", "ContactName");
        }
    }
}
