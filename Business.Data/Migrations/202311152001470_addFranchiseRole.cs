namespace BusinessData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addFranchiseRole : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FranchiseRole",
                c => new
                    {
                        FranchiseRoleId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Email = c.String(),
                        Phone = c.String(),
                        Role = c.Int(nullable: false),
                        FranchiseId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.FranchiseRoleId)
                .ForeignKey("dbo.Franchise", t => t.FranchiseId, cascadeDelete: true)
                .Index(t => t.FranchiseId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FranchiseRole", "FranchiseId", "dbo.Franchise");
            DropIndex("dbo.FranchiseRole", new[] { "FranchiseId" });
            DropTable("dbo.FranchiseRole");
        }
    }
}
