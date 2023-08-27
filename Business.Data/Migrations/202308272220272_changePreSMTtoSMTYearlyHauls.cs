namespace BusinessData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changePreSMTtoSMTYearlyHauls : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Client", "HaulsPerWeek", c => c.Single(nullable: false));
            DropColumn("dbo.Client", "HaulsPerDay");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Client", "HaulsPerDay", c => c.Single(nullable: false));
            DropColumn("dbo.Client", "HaulsPerWeek");
        }
    }
}
