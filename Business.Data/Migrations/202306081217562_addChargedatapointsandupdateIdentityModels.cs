namespace BusinessData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addChargedatapointsandupdateIdentityModels : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.WorkOrder", "FranchiseId", "dbo.Franchise");
            DropForeignKey("dbo.WorkOrder", "InvoiceId", "dbo.Invoice");
            AddColumn("dbo.WOCharges", "VonigoChargeId", c => c.Int(nullable: false));
            AddColumn("dbo.WOCharges", "ItemType", c => c.String());
            AddColumn("dbo.WOCharges", "VonigoInvoiceId", c => c.Int(nullable: false));
            AddColumn("dbo.WOCharges", "Title", c => c.String());
            AddColumn("dbo.WOCharges", "CreatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.WOCharges", "EditedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.WOCharges", "ActiveCan", c => c.Boolean(nullable: false));
            AddColumn("dbo.WOCharges", "Quantity", c => c.Int(nullable: false));
            AddColumn("dbo.WOCharges", "VonigoFranchiseId", c => c.Int(nullable: false));
            AddColumn("dbo.WOCharges", "FranchiseId", c => c.Int(nullable: false));
            CreateIndex("dbo.WOCharges", "FranchiseId");
            AddForeignKey("dbo.WOCharges", "FranchiseId", "dbo.Franchise", "FranchiseId");
            AddForeignKey("dbo.WorkOrder", "FranchiseId", "dbo.Franchise", "FranchiseId");
            AddForeignKey("dbo.WorkOrder", "InvoiceId", "dbo.Invoice", "InvoiceId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WorkOrder", "InvoiceId", "dbo.Invoice");
            DropForeignKey("dbo.WorkOrder", "FranchiseId", "dbo.Franchise");
            DropForeignKey("dbo.WOCharges", "FranchiseId", "dbo.Franchise");
            DropIndex("dbo.WOCharges", new[] { "FranchiseId" });
            DropColumn("dbo.WOCharges", "FranchiseId");
            DropColumn("dbo.WOCharges", "VonigoFranchiseId");
            DropColumn("dbo.WOCharges", "Quantity");
            DropColumn("dbo.WOCharges", "ActiveCan");
            DropColumn("dbo.WOCharges", "EditedDate");
            DropColumn("dbo.WOCharges", "CreatedDate");
            DropColumn("dbo.WOCharges", "Title");
            DropColumn("dbo.WOCharges", "VonigoInvoiceId");
            DropColumn("dbo.WOCharges", "ItemType");
            DropColumn("dbo.WOCharges", "VonigoChargeId");
            AddForeignKey("dbo.WorkOrder", "InvoiceId", "dbo.Invoice", "InvoiceId", cascadeDelete: true);
            AddForeignKey("dbo.WorkOrder", "FranchiseId", "dbo.Franchise", "FranchiseId", cascadeDelete: true);
        }
    }
}
