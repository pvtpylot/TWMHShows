using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MauiBlazorWeb.Web.Migrations
{
    /// <inheritdoc />
    public partial class ChangesAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "HeroShotImage",
                table: "UserModelObjects",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ShowImage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImageData = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    UserModelObjectId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShowImage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShowImage_UserModelObjects_UserModelObjectId",
                        column: x => x.UserModelObjectId,
                        principalTable: "UserModelObjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShowImage_UserModelObjectId",
                table: "ShowImage",
                column: "UserModelObjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShowImage");

            migrationBuilder.DropColumn(
                name: "HeroShotImage",
                table: "UserModelObjects");
        }
    }
}
