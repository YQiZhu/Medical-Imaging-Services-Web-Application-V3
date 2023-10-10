namespace FIT5032_PortfolioV3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateAppointmentModel : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Appointments", name: "StaffUserId", newName: "__mig_tmp__0");
            RenameColumn(table: "dbo.Appointments", name: "PatientUserId", newName: "StaffUserId");
            RenameColumn(table: "dbo.Appointments", name: "__mig_tmp__0", newName: "PatientUserId");
            RenameIndex(table: "dbo.Appointments", name: "IX_StaffUserId", newName: "__mig_tmp__0");
            RenameIndex(table: "dbo.Appointments", name: "IX_PatientUserId", newName: "IX_StaffUserId");
            RenameIndex(table: "dbo.Appointments", name: "__mig_tmp__0", newName: "IX_PatientUserId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Appointments", name: "IX_PatientUserId", newName: "__mig_tmp__0");
            RenameIndex(table: "dbo.Appointments", name: "IX_StaffUserId", newName: "IX_PatientUserId");
            RenameIndex(table: "dbo.Appointments", name: "__mig_tmp__0", newName: "IX_StaffUserId");
            RenameColumn(table: "dbo.Appointments", name: "PatientUserId", newName: "__mig_tmp__0");
            RenameColumn(table: "dbo.Appointments", name: "StaffUserId", newName: "PatientUserId");
            RenameColumn(table: "dbo.Appointments", name: "__mig_tmp__0", newName: "StaffUserId");
        }
    }
}
