using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeFlex.Migrations
{
    /// <inheritdoc />
    public partial class again : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBooked",
                table: "Properties",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UsersId",
                table: "Properties",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Properties_UsersId",
                table: "Properties",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_PropertyId",
                table: "Locations",
                column: "PropertyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Images_PropertyId",
                table: "Images",
                column: "PropertyId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Properties_PropertyId",
                table: "Images",
                column: "PropertyId",
                principalTable: "Properties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Locations_Properties_PropertyId",
                table: "Locations",
                column: "PropertyId",
                principalTable: "Properties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_AspNetUsers_UsersId",
                table: "Properties",
                column: "UsersId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Properties_PropertyId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Locations_Properties_PropertyId",
                table: "Locations");

            migrationBuilder.DropForeignKey(
                name: "FK_Properties_AspNetUsers_UsersId",
                table: "Properties");

            migrationBuilder.DropIndex(
                name: "IX_Properties_UsersId",
                table: "Properties");

            migrationBuilder.DropIndex(
                name: "IX_Locations_PropertyId",
                table: "Locations");

            migrationBuilder.DropIndex(
                name: "IX_Images_PropertyId",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "IsBooked",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "UsersId",
                table: "Properties");
        }
    }
}
