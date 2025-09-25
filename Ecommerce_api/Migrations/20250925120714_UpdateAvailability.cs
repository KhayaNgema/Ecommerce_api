using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce_api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAvailability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Availability",
                table: "Products");
        }
    }
}
