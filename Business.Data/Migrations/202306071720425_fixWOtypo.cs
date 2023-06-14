namespace BusinessData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixWOtypo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WorkOrder", "BeforeFullness", c => c.Int(nullable: false));
            DropColumn("dbo.WorkOrder", "BefofeFullness");
        }
        
        public override void Down()
        {
            AddColumn("dbo.WorkOrder", "BefofeFullness", c => c.Int(nullable: false));
            DropColumn("dbo.WorkOrder", "BeforeFullness");
        }
    }
}
