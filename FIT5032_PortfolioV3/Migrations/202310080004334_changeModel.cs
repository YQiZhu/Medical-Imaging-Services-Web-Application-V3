namespace FIT5032_PortfolioV3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clinics", "AddressDetail", c => c.String(nullable: false));
            DropColumn("dbo.Clinics", "ddressDetail");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Clinics", "ddressDetail", c => c.String(nullable: false));
            DropColumn("dbo.Clinics", "AddressDetail");
        }
    }
}
