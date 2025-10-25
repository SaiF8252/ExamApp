namespace ExamApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class first : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Products", "Stock");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Products", "Stock", c => c.Int(nullable: false));
        }
    }
}
