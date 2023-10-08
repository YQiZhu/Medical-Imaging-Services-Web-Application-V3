namespace FIT5032_PortfolioV3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addDoBModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "DoB", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "DoB");
        }
    }
}
