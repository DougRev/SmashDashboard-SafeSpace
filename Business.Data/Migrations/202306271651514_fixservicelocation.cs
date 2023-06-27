namespace BusinessData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixservicelocation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Client", "ServiceLocation", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Client", "ServiceLocation");
        }
    }
}
