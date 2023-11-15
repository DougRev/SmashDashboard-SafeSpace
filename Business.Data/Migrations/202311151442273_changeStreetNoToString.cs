namespace BusinessData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeStreetNoToString : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Client", "StreetNumber", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Client", "StreetNumber", c => c.Int());
        }
    }
}
