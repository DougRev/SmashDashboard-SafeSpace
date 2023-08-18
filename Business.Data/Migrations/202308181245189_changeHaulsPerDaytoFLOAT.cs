namespace BusinessData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeHaulsPerDaytoFLOAT : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Client", "HaulsPerDay", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Client", "HaulsPerDay", c => c.Int(nullable: false));
        }
    }
}
