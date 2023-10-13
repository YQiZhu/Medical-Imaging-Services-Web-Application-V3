namespace FIT5032_PortfolioV3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateRatingModel2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Ratings", "Description", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Ratings", "Description", c => c.String(nullable: false));
        }
    }
}
