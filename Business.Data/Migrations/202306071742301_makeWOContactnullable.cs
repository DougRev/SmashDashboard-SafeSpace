namespace BusinessData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class makeWOContactnullable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.WorkOrder", "ContactId", "dbo.Contact");
            DropIndex("dbo.WorkOrder", new[] { "ContactId" });
            AlterColumn("dbo.WorkOrder", "ContactId", c => c.Int());
            CreateIndex("dbo.WorkOrder", "ContactId");
            AddForeignKey("dbo.WorkOrder", "ContactId", "dbo.Contact", "ContactId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WorkOrder", "ContactId", "dbo.Contact");
            DropIndex("dbo.WorkOrder", new[] { "ContactId" });
            AlterColumn("dbo.WorkOrder", "ContactId", c => c.Int(nullable: false));
            CreateIndex("dbo.WorkOrder", "ContactId");
            AddForeignKey("dbo.WorkOrder", "ContactId", "dbo.Contact", "ContactId", cascadeDelete: true);
        }
    }
}
