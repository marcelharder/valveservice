using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ValveService.Migrations
{
    /// <inheritdoc />
    public partial class JsonUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "ConeAngle",
                table: "ValveSizes",
                type: "float",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "Height",
                table: "ValveSizes",
                type: "float",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "IOD",
                table: "ValveSizes",
                type: "float",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "OOD",
                table: "ValveSizes",
                type: "float",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConeAngle",
                table: "ValveSizes");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "ValveSizes");

            migrationBuilder.DropColumn(
                name: "IOD",
                table: "ValveSizes");

            migrationBuilder.DropColumn(
                name: "OOD",
                table: "ValveSizes");
        }
    }
}
