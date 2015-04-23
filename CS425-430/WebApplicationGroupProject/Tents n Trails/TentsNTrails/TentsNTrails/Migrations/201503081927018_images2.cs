namespace TentsNTrails.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class images2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.LocationLocationImages", "Location_LocationID", "dbo.Locations");
            DropForeignKey("dbo.LocationLocationImages", "LocationImage_ImageID", "dbo.Images");
            DropIndex("dbo.LocationLocationImages", new[] { "Location_LocationID" });
            DropIndex("dbo.LocationLocationImages", new[] { "LocationImage_ImageID" });
            CreateIndex("dbo.Images", "LocationID");
            AddForeignKey("dbo.Images", "LocationID", "dbo.Locations", "LocationID", cascadeDelete: true);
            DropTable("dbo.LocationLocationImages");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.LocationLocationImages",
                c => new
                    {
                        Location_LocationID = c.Int(nullable: false),
                        LocationImage_ImageID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Location_LocationID, t.LocationImage_ImageID });
            
            DropForeignKey("dbo.Images", "LocationID", "dbo.Locations");
            DropIndex("dbo.Images", new[] { "LocationID" });
            CreateIndex("dbo.LocationLocationImages", "LocationImage_ImageID");
            CreateIndex("dbo.LocationLocationImages", "Location_LocationID");
            AddForeignKey("dbo.LocationLocationImages", "LocationImage_ImageID", "dbo.Images", "ImageID", cascadeDelete: true);
            AddForeignKey("dbo.LocationLocationImages", "Location_LocationID", "dbo.Locations", "LocationID", cascadeDelete: true);
        }
    }
}
