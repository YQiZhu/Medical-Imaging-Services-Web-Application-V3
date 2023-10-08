namespace FIT5032_PortfolioV3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addRatingReportModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Ratings",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Description = c.String(nullable: false),
                        Date = c.String(nullable: false),
                        Time = c.String(nullable: false),
                        AppointmentId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Appointments", t => t.AppointmentId, cascadeDelete: true)
                .Index(t => t.AppointmentId);
            
            CreateTable(
                "dbo.Reports",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Description = c.String(nullable: false),
                        Date = c.String(nullable: false),
                        Time = c.String(nullable: false),
                        AppointmentId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Appointments", t => t.AppointmentId, cascadeDelete: true)
                .Index(t => t.AppointmentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Reports", "AppointmentId", "dbo.Appointments");
            DropForeignKey("dbo.Ratings", "AppointmentId", "dbo.Appointments");
            DropIndex("dbo.Reports", new[] { "AppointmentId" });
            DropIndex("dbo.Ratings", new[] { "AppointmentId" });
            DropTable("dbo.Reports");
            DropTable("dbo.Ratings");
        }
    }
}
