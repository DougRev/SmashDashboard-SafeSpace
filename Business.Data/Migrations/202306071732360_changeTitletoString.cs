namespace BusinessData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeTitletoString : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.WorkOrder", "Title", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.WorkOrder", "Title", c => c.Int(nullable: false));
        }
    }
}
