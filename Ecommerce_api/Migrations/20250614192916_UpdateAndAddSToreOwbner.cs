using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce_api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAndAddSToreOwbner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Stores_StoreId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_StoreId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "StoreAddress",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "AssociateNumber",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Stores",
                newName: "Address");

            migrationBuilder.AlterColumn<string>(
                name: "StoreCode",
                table: "Stores",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Stores",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Stores",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StoreOwnerId",
                table: "Stores",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stores_StoreOwnerId",
                table: "Stores",
                column: "StoreOwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Stores_AspNetUsers_StoreOwnerId",
                table: "Stores",
                column: "StoreOwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stores_AspNetUsers_StoreOwnerId",
                table: "Stores");

            migrationBuilder.DropIndex(
                name: "IX_Stores_StoreOwnerId",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "StoreOwnerId",
                table: "Stores");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "Stores",
                newName: "UserId");

            migrationBuilder.AlterColumn<int>(
                name: "StoreCode",
                table: "Stores",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "StoreAddress",
                table: "Stores",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AssociateNumber",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StoreId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_StoreId",
                table: "AspNetUsers",
                column: "StoreId",
                unique: true,
                filter: "[StoreId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Stores_StoreId",
                table: "AspNetUsers",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "StoreId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
