namespace FIT5032_PortfolioV3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateDatabaseModel1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Appointments", "TimeSlotId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Appointments", "TimeSlotId");
            AddForeignKey("dbo.Appointments", "TimeSlotId", "dbo.TimeSlots", "SlotId", cascadeDelete: true);
            DropColumn("dbo.Appointments", "Time");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Appointments", "Time", c => c.String(nullable: false));
            DropForeignKey("dbo.Appointments", "TimeSlotId", "dbo.TimeSlots");
            DropIndex("dbo.Appointments", new[] { "TimeSlotId" });
            DropColumn("dbo.Appointments", "TimeSlotId");
        }
    }
}
