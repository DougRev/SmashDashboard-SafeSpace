namespace BusinessData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addVonigoWorkOrderdataPoints : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WorkOrder", "VonigoWorkOrderId", c => c.Int(nullable: false));
            AddColumn("dbo.WorkOrder", "VonigoClientId", c => c.Int(nullable: false));
            AddColumn("dbo.WorkOrder", "Title", c => c.Int(nullable: false));
            AddColumn("dbo.WorkOrder", "CompletedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.WorkOrder", "CompletedTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.WorkOrder", "VonigoIsActive", c => c.Boolean(nullable: false));
            AddColumn("dbo.WorkOrder", "Summary", c => c.String());
            AddColumn("dbo.WorkOrder", "OnSiteContact", c => c.String());
            AddColumn("dbo.WorkOrder", "ServiceType", c => c.String());
            AddColumn("dbo.WorkOrder", "VonigoLabel", c => c.String());
            AddColumn("dbo.WorkOrder", "VonigoInvoiceId", c => c.Int(nullable: false));
            AddColumn("dbo.WorkOrder", "BefofeFullness", c => c.Int(nullable: false));
            AddColumn("dbo.WorkOrder", "AfterFullness", c => c.Int(nullable: false));
            DropColumn("dbo.WorkOrder", "CompletedDateTime");
        }
        
        public override void Down()
        {
            AddColumn("dbo.WorkOrder", "CompletedDateTime", c => c.DateTime(nullable: false));
            DropColumn("dbo.WorkOrder", "AfterFullness");
            DropColumn("dbo.WorkOrder", "BefofeFullness");
            DropColumn("dbo.WorkOrder", "VonigoInvoiceId");
            DropColumn("dbo.WorkOrder", "VonigoLabel");
            DropColumn("dbo.WorkOrder", "ServiceType");
            DropColumn("dbo.WorkOrder", "OnSiteContact");
            DropColumn("dbo.WorkOrder", "Summary");
            DropColumn("dbo.WorkOrder", "VonigoIsActive");
            DropColumn("dbo.WorkOrder", "CompletedTime");
            DropColumn("dbo.WorkOrder", "CompletedDate");
            DropColumn("dbo.WorkOrder", "Title");
            DropColumn("dbo.WorkOrder", "VonigoClientId");
            DropColumn("dbo.WorkOrder", "VonigoWorkOrderId");
        }
    }
}
