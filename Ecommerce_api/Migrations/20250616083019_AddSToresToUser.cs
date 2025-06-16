using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce_api.Migrations
{
    /// <inheritdoc />
    public partial class AddSToresToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop FK and index from AspNetUsers.StoreId to Stores
/*            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Stores_StoreId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_StoreId",
                table: "AspNetUsers");*/

            // Drop FK and index from Stores.StoreOwnerId to AspNetUsers
            migrationBuilder.DropForeignKey(
                name: "FK_Stores_AspNetUsers_StoreOwnerId",
                table: "Stores");

            migrationBuilder.DropIndex(
                name: "IX_Stores_StoreOwnerId",
                table: "Stores");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Recreate index and FK from AspNetUsers.StoreId to Stores
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

            // Recreate index and FK from Stores.StoreOwnerId to AspNetUsers
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
    }
}
