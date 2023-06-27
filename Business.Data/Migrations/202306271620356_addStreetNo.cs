namespace BusinessData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addStreetNo : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Client", "StreetNumber", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Client", "StreetNumber", c => c.String());
        }
    }
}
