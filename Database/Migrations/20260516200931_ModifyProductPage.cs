using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PriceTrail.Database.Migrations
{
    /// <inheritdoc />
    public partial class ModifyProductPage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasError",
                table: "ProductPages",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasError",
                table: "ProductPages");
        }
    }
}
