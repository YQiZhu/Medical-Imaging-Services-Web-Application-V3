namespace FIT5032_PortfolioV3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateClinicModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clinics", "AverageRate", c => c.Decimal(nullable: false, precision: 2, scale: 1, storeType: "numeric"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Clinics", "AverageRate");
        }
    }
}
