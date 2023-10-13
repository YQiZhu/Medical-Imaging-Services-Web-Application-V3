namespace FIT5032_PortfolioV3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateRatingModel1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Ratings", "Rate", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Ratings", "Rate", c => c.String(nullable: false));
        }
    }
}
