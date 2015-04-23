namespace TentsNTrails.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddLocationFlagModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LocationFlags",
                c => new
                {
                    FlagID = c.Int(nullable: false, identity: true),
                    LocationID = c.Int(nullable: false),
                    Flag = c.Int(nullable: false),
                    User_Id = c.String(maxLength: 128),
                })
                .PrimaryKey(t => t.FlagID)
                .ForeignKey("dbo.Locations", t => t.LocationID, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.LocationID)
                .Index(t => t.User_Id);

        }

        public override void Down()
        {
            DropForeignKey("dbo.LocationFlags", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.LocationFlags", "LocationID", "dbo.Locations");
            DropIndex("dbo.LocationFlags", new[] { "User_Id" });
            DropIndex("dbo.LocationFlags", new[] { "LocationID" });
            DropTable("dbo.LocationFlags");
        }
    }
}
