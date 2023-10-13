namespace FIT5032_PortfolioV3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateRatingModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Ratings", "Rate", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Ratings", "Rate");
        }
    }
}
