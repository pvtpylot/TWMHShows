using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MauiBlazorWeb.Web.Migrations
{
    /// <inheritdoc />
    public partial class NameAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "First",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Last",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "First",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Last",
                table: "AspNetUsers");
        }
    }
}
