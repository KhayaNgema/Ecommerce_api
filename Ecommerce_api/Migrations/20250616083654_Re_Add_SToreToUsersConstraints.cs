using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce_api.Migrations
{
    /// <inheritdoc />
    public partial class Re_Add_SToreToUsersConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stores_AspNetUsers_CreatedById",
                table: "Stores");

            migrationBuilder.DropForeignKey(
                name: "FK_Stores_AspNetUsers_ModifiedById",
                table: "Stores");

            migrationBuilder.AddForeignKey(
                name: "FK_Stores_AspNetUsers_CreatedById",
                table: "Stores",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Stores_AspNetUsers_ModifiedById",
                table: "Stores",
                column: "ModifiedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.CreateIndex(
                  name: "IX_Stores_StoreOwnerId",
                  table: "Stores",
                  column: "StoreOwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Stores_AspNetUsers_StoreOwnerId",
                table: "Stores",
                column: "StoreOwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stores_AspNetUsers_CreatedById",
                table: "Stores");

            migrationBuilder.DropForeignKey(
                name: "FK_Stores_AspNetUsers_ModifiedById",
                table: "Stores");

            migrationBuilder.AddForeignKey(
                name: "FK_Stores_AspNetUsers_CreatedById",
                table: "Stores",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Stores_AspNetUsers_ModifiedById",
                table: "Stores",
                column: "ModifiedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);



        }
    }
}
