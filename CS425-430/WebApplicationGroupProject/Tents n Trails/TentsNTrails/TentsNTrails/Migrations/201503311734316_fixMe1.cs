namespace TentsNTrails.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixMe1 : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.LocationRecreations", "RecreationID");
            AddForeignKey("dbo.LocationRecreations", "RecreationID", "dbo.Recreations", "RecreationID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LocationRecreations", "RecreationID", "dbo.Recreations");
            DropIndex("dbo.LocationRecreations", new[] { "RecreationID" });
        }
    }
}
