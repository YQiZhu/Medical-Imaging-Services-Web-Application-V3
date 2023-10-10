namespace FIT5032_PortfolioV3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixingAppointmentModel : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Appointments", "Description", c => c.String());
            AlterColumn("dbo.Appointments", "RoomNo", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Appointments", "RoomNo", c => c.String(nullable: false));
            AlterColumn("dbo.Appointments", "Description", c => c.String(nullable: false));
        }
    }
}
