using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PPTApi.Migrations
{
    public partial class addDriveFilePathAndIdColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DriveFileId",
                table: "PptDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DriveFilepath",
                table: "PptDetails",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DriveFileId",
                table: "PptDetails");

            migrationBuilder.DropColumn(
                name: "DriveFilepath",
                table: "PptDetails");
        }
    }
}
