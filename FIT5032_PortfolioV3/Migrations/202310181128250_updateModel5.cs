namespace FIT5032_PortfolioV3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateModel5 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WorkClinics",
                c => new
                    {
                        ClinicId = c.String(nullable: false, maxLength: 128),
                        StaffId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.ClinicId, t.StaffId });
            
        }
        
        public override void Down()
        {
            DropTable("dbo.WorkClinics");
        }
    }
}
