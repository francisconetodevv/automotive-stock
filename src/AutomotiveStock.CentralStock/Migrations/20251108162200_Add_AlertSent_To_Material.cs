using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutomotiveStock.CentralStock.Migrations
{
    /// <inheritdoc />
    public partial class Add_AlertSent_To_Material : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AlertSent",
                table: "Materials",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlertSent",
                table: "Materials");
        }
    }
}
