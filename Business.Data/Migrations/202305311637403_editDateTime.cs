namespace BusinessData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class editDateTime : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Franchise", "LaunchDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Franchise", "LaunchDate", c => c.DateTime(nullable: false));
        }
    }
}
