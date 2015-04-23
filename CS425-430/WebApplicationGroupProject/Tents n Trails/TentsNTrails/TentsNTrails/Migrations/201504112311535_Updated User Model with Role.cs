namespace TentsNTrails.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedUserModelwithRole : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetRoles", "User_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.AspNetRoles", "User_Id");
            AddForeignKey("dbo.AspNetRoles", "User_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetRoles", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.AspNetRoles", new[] { "User_Id" });
            DropColumn("dbo.AspNetRoles", "User_Id");
        }
    }
}
