using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PriceTrail.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddLaunchOnStartupSetting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "LaunchOnStartupEnabled",
                table: "Settings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LaunchOnStartupEnabled",
                table: "Settings");
        }
    }
}
