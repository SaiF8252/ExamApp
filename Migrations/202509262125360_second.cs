namespace ExamApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class second : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "StockQuantity", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "StockQuantity");
        }
    }
}
