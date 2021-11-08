using Microsoft.EntityFrameworkCore.Migrations;

namespace Jobsity.Chatroom.Data.Migrations
{
    public partial class AlterTable_Message_Add_ChatroomId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChatroomId",
                table: "Message",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChatroomId",
                table: "Message");
        }
    }
}
