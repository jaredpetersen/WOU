namespace TentsNTrails.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class difficultyrating : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Locations", "Difficulty", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Locations", "Difficulty");
        }
    }
}
