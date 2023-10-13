namespace FIT5032_PortfolioV3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateModel4 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Appointments", "Description");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Appointments", "Description", c => c.String());
        }
    }
}
