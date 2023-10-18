namespace FIT5032_PortfolioV3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateDatabaseModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BookedSlots",
                c => new
                    {
                        BookingId = c.String(nullable: false, maxLength: 128),
                        SlotId = c.String(maxLength: 128),
                        StaffUserId = c.String(),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.BookingId)
                .ForeignKey("dbo.TimeSlots", t => t.SlotId)
                .Index(t => t.SlotId);
            
            CreateTable(
                "dbo.TimeSlots",
                c => new
                    {
                        SlotId = c.String(nullable: false, maxLength: 128),
                        StartTime = c.Time(nullable: false, precision: 7),
                        EndTime = c.Time(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.SlotId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BookedSlots", "SlotId", "dbo.TimeSlots");
            DropIndex("dbo.BookedSlots", new[] { "SlotId" });
            DropTable("dbo.TimeSlots");
            DropTable("dbo.BookedSlots");
        }
    }
}
