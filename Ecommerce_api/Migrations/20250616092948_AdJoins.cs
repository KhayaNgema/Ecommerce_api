using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce_api.Migrations
{
    /// <inheritdoc />
    public partial class AdJoins : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoreOwnerStore_AspNetUsers_StoreOwnerId",
                table: "StoreOwnerStore");

            migrationBuilder.DropForeignKey(
                name: "FK_StoreOwnerStore_Stores_StoreId",
                table: "StoreOwnerStore");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StoreOwnerStore",
                table: "StoreOwnerStore");

            migrationBuilder.RenameTable(
                name: "StoreOwnerStore",
                newName: "StoreOwnerStores");

            migrationBuilder.RenameIndex(
                name: "IX_StoreOwnerStore_StoreId",
                table: "StoreOwnerStores",
                newName: "IX_StoreOwnerStores_StoreId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StoreOwnerStores",
                table: "StoreOwnerStores",
                columns: new[] { "StoreOwnerId", "StoreId" });

            migrationBuilder.AddForeignKey(
                name: "FK_StoreOwnerStores_AspNetUsers_StoreOwnerId",
                table: "StoreOwnerStores",
                column: "StoreOwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_StoreOwnerStores_Stores_StoreId",
                table: "StoreOwnerStores",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "StoreId",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoreOwnerStores_AspNetUsers_StoreOwnerId",
                table: "StoreOwnerStores");

            migrationBuilder.DropForeignKey(
                name: "FK_StoreOwnerStores_Stores_StoreId",
                table: "StoreOwnerStores");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StoreOwnerStores",
                table: "StoreOwnerStores");

            migrationBuilder.RenameTable(
                name: "StoreOwnerStores",
                newName: "StoreOwnerStore");

            migrationBuilder.RenameIndex(
                name: "IX_StoreOwnerStores_StoreId",
                table: "StoreOwnerStore",
                newName: "IX_StoreOwnerStore_StoreId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StoreOwnerStore",
                table: "StoreOwnerStore",
                columns: new[] { "StoreOwnerId", "StoreId" });

            migrationBuilder.AddForeignKey(
                name: "FK_StoreOwnerStore_AspNetUsers_StoreOwnerId",
                table: "StoreOwnerStore",
                column: "StoreOwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StoreOwnerStore_Stores_StoreId",
                table: "StoreOwnerStore",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "StoreId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
