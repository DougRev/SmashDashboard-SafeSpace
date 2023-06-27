namespace BusinessData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeVonigoLabel : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.WorkOrder", "VonigoLabel");
        }
        
        public override void Down()
        {
            AddColumn("dbo.WorkOrder", "VonigoLabel", c => c.String());
        }
    }
}
