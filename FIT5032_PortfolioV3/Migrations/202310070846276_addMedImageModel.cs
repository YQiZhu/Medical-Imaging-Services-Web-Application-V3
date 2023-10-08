namespace FIT5032_PortfolioV3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addMedImageModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MedImages",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
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
            DropForeignKey("dbo.MedImages", "AppointmentId", "dbo.Appointments");
            DropIndex("dbo.MedImages", new[] { "AppointmentId" });
            DropTable("dbo.MedImages");
        }
    }
}
