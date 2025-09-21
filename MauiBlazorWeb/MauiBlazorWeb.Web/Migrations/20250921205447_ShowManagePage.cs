using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MauiBlazorWeb.Web.Migrations
{
    /// <inheritdoc />
    public partial class ShowManagePage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdditionalMetadata",
                table: "Shows",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AllowMemberOnlyEntries",
                table: "Shows",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsNanQualifying",
                table: "Shows",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPrivate",
                table: "Shows",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "NamhsaGuidelines",
                table: "Shows",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShowFormat",
                table: "Shows",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ShowType",
                table: "Shows",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AgeRestriction",
                table: "ShowClasses",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreedCategory",
                table: "ShowClasses",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CollectibilityType",
                table: "ShowClasses",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ColorRestriction",
                table: "ShowClasses",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FinishType",
                table: "ShowClasses",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GenderRestriction",
                table: "ShowClasses",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PerformanceType",
                table: "ShowClasses",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ScaleRestriction",
                table: "ShowClasses",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "ShowClasses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Divisions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "DivisionType",
                table: "Divisions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "Divisions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalMetadata",
                table: "Shows");

            migrationBuilder.DropColumn(
                name: "AllowMemberOnlyEntries",
                table: "Shows");

            migrationBuilder.DropColumn(
                name: "IsNanQualifying",
                table: "Shows");

            migrationBuilder.DropColumn(
                name: "IsPrivate",
                table: "Shows");

            migrationBuilder.DropColumn(
                name: "NamhsaGuidelines",
                table: "Shows");

            migrationBuilder.DropColumn(
                name: "ShowFormat",
                table: "Shows");

            migrationBuilder.DropColumn(
                name: "ShowType",
                table: "Shows");

            migrationBuilder.DropColumn(
                name: "AgeRestriction",
                table: "ShowClasses");

            migrationBuilder.DropColumn(
                name: "BreedCategory",
                table: "ShowClasses");

            migrationBuilder.DropColumn(
                name: "CollectibilityType",
                table: "ShowClasses");

            migrationBuilder.DropColumn(
                name: "ColorRestriction",
                table: "ShowClasses");

            migrationBuilder.DropColumn(
                name: "FinishType",
                table: "ShowClasses");

            migrationBuilder.DropColumn(
                name: "GenderRestriction",
                table: "ShowClasses");

            migrationBuilder.DropColumn(
                name: "PerformanceType",
                table: "ShowClasses");

            migrationBuilder.DropColumn(
                name: "ScaleRestriction",
                table: "ShowClasses");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "ShowClasses");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Divisions");

            migrationBuilder.DropColumn(
                name: "DivisionType",
                table: "Divisions");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "Divisions");
        }
    }
}
