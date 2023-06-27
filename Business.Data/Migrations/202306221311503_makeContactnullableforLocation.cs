namespace BusinessData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class makeContactnullableforLocation : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Location", "ContactId", "dbo.Contact");
            DropIndex("dbo.Location", new[] { "ContactId" });
            AlterColumn("dbo.Location", "ContactId", c => c.Int());
            CreateIndex("dbo.Location", "ContactId");
            AddForeignKey("dbo.Location", "ContactId", "dbo.Contact", "ContactId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Location", "ContactId", "dbo.Contact");
            DropIndex("dbo.Location", new[] { "ContactId" });
            AlterColumn("dbo.Location", "ContactId", c => c.Int(nullable: false));
            CreateIndex("dbo.Location", "ContactId");
            AddForeignKey("dbo.Location", "ContactId", "dbo.Contact", "ContactId", cascadeDelete: true);
        }
    }
}
