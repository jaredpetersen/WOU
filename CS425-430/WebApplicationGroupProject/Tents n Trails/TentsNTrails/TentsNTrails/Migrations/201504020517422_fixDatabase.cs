namespace TentsNTrails.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixDatabase : DbMigration
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
                .ForeignKey("dbo.Recreations", t => t.RecreationID, cascadeDelete: true)
                .Index(t => t.LocationID)
                .Index(t => t.RecreationID);
            
            //AddColumn("dbo.Locations", "Description", c => c.String(maxLength: 250));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LocationRecreations", "RecreationID", "dbo.Recreations");
            DropForeignKey("dbo.LocationRecreations", "LocationID", "dbo.Locations");
            DropIndex("dbo.LocationRecreations", new[] { "RecreationID" });
            DropIndex("dbo.LocationRecreations", new[] { "LocationID" });
            DropColumn("dbo.Locations", "Description");
            DropTable("dbo.LocationRecreations");
        }
    }
}
