using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MauiBlazorWeb.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddShowHolderRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                values: new object[] 
                { 
                    Guid.NewGuid().ToString(), 
                    "ShowHolder", 
                    "SHOWHOLDER", 
                    Guid.NewGuid().ToString() 
                });

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Name",
                keyValue: "ShowHolder");

        }
    }
}
