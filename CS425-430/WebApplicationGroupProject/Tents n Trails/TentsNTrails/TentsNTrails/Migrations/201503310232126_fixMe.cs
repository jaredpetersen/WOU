namespace TentsNTrails.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixMe : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LocationRecreations",
                c => new
                    {
                        LocationID = c.Int(nullable: false),
                        RecreationID = c.Int(nullable: false),
                        RecreationLabel = c.String(),
                        IsChecked = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.LocationID, t.RecreationID })
                .ForeignKey("dbo.Locations", t => t.LocationID, cascadeDelete: true)
                .Index(t => t.LocationID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LocationRecreations", "LocationID", "dbo.Locations");
            DropIndex("dbo.LocationRecreations", new[] { "LocationID" });
            DropTable("dbo.LocationRecreations");
        }
    }
}
