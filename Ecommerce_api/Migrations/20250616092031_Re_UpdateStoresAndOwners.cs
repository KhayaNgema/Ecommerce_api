using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce_api.Migrations
{
    /// <inheritdoc />
    public partial class Re_UpdateStoresAndOwners : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stores_AspNetUsers_StoreOwnerId",
                table: "Stores");

            migrationBuilder.DropIndex(
                name: "IX_Stores_StoreOwnerId",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "StoreOwnerId",
                table: "Stores");

            migrationBuilder.CreateTable(
                name: "StoreOwnerStore",
                columns: table => new
                {
                    StoreOwnerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreOwnerStore", x => new { x.StoreOwnerId, x.StoreId });
                    table.ForeignKey(
                        name: "FK_StoreOwnerStore_AspNetUsers_StoreOwnerId",
                        column: x => x.StoreOwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_StoreOwnerStore_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StoreOwnerStore_StoreId",
                table: "StoreOwnerStore",
                column: "StoreId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StoreOwnerStore");

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
    }
}
