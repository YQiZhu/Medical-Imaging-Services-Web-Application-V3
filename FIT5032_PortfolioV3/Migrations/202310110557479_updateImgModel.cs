namespace FIT5032_PortfolioV3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateImgModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MedImages", "Path", c => c.String(nullable: false, maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MedImages", "Path");
        }
    }
}
