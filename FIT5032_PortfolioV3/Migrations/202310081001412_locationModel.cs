namespace FIT5032_PortfolioV3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class locationModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clinics", "Description", c => c.String());
            AddColumn("dbo.Clinics", "Latitude", c => c.Decimal(nullable: false, precision: 10, scale: 8, storeType: "numeric"));
            AddColumn("dbo.Clinics", "Longitude", c => c.Decimal(nullable: false, precision: 11, scale: 8, storeType: "numeric"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Clinics", "Longitude");
            DropColumn("dbo.Clinics", "Latitude");
            DropColumn("dbo.Clinics", "Description");
        }
    }
}
