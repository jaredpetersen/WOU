namespace TentsNTrails.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeCaption : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Images", "Caption");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Images", "Caption", c => c.String());
        }
    }
}
