using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PriceTrail.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddErrorMessageToProductPage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ErrorMessage",
                table: "ProductPages",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ErrorMessage",
                table: "ProductPages");
        }
    }
}
