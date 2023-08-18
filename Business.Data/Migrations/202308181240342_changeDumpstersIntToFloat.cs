namespace BusinessData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeDumpstersIntToFloat : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Client", "NumberOfDumpsters", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Client", "NumberOfDumpsters", c => c.Int(nullable: false));
        }
    }
}
