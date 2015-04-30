namespace TentsNTrails.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addProfileVideos : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Videos", "User_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Videos", "User_Id");
            AddForeignKey("dbo.Videos", "User_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Videos", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Videos", new[] { "User_Id" });
            DropColumn("dbo.Videos", "User_Id");
        }
    }
}
