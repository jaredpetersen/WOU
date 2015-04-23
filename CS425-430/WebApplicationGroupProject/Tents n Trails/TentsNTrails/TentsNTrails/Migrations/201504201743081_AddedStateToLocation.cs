namespace TentsNTrails.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedStateToLocation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Locations", "State", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Locations", "State");
        }
    }
}
