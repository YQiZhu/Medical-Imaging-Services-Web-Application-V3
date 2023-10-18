namespace FIT5032_PortfolioV3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateDatabaseModel2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.BookedSlots", "Date", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.BookedSlots", "Date", c => c.DateTime(nullable: false));
        }
    }
}
