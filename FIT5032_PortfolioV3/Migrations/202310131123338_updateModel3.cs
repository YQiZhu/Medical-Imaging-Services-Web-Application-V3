namespace FIT5032_PortfolioV3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateModel3 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Appointments", "Time", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Appointments", "Time", c => c.Time(nullable: false, precision: 7));
        }
    }
}
