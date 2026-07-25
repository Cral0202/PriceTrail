using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PriceTrail.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationsSetting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MinimizeToTray",
                table: "Settings",
                newName: "NotificationsEnabled");

            migrationBuilder.AddColumn<bool>(
                name: "MinimizeToTrayEnabled",
                table: "Settings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinimizeToTrayEnabled",
                table: "Settings");

            migrationBuilder.RenameColumn(
                name: "NotificationsEnabled",
                table: "Settings",
                newName: "MinimizeToTray");
        }
    }
}
