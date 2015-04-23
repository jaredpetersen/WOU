namespace TentsNTrails.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class images : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Images",
                c => new
                    {
                        ImageID = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        AltText = c.String(),
                        Caption = c.String(),
                        ImageUrl = c.String(nullable: false),
                        DateTaken = c.DateTime(),
                        DateCreated = c.DateTime(),
                        DateModified = c.DateTime(),
                        LocationID = c.Int(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ImageID)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.LocationLocationImages",
                c => new
                    {
                        Location_LocationID = c.Int(nullable: false),
                        LocationImage_ImageID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Location_LocationID, t.LocationImage_ImageID })
                .ForeignKey("dbo.Locations", t => t.Location_LocationID, cascadeDelete: true)
                .ForeignKey("dbo.Images", t => t.LocationImage_ImageID, cascadeDelete: true)
                .Index(t => t.Location_LocationID)
                .Index(t => t.LocationImage_ImageID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LocationLocationImages", "LocationImage_ImageID", "dbo.Images");
            DropForeignKey("dbo.LocationLocationImages", "Location_LocationID", "dbo.Locations");
            DropForeignKey("dbo.Images", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.LocationLocationImages", new[] { "LocationImage_ImageID" });
            DropIndex("dbo.LocationLocationImages", new[] { "Location_LocationID" });
            DropIndex("dbo.Images", new[] { "User_Id" });
            DropTable("dbo.LocationLocationImages");
            DropTable("dbo.Images");
        }
    }
}
