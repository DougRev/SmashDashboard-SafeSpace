namespace BusinessData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeStreetNOfromClient : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Location", "StreetNumber");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Location", "StreetNumber", c => c.String());
        }
    }
}
