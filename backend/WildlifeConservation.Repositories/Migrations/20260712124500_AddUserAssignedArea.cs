using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WildlifeConservation.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddUserAssignedArea : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssignedLocationName",
                table: "Users",
                type: "character varying(160)",
                maxLength: 160,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AssignedLatitude",
                table: "Users",
                type: "numeric(9,6)",
                precision: 9,
                scale: 6,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AssignedLongitude",
                table: "Users",
                type: "numeric(9,6)",
                precision: 9,
                scale: 6,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AssignedMapZoom",
                table: "Users",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "AssignedLocationName", table: "Users");
            migrationBuilder.DropColumn(name: "AssignedLatitude", table: "Users");
            migrationBuilder.DropColumn(name: "AssignedLongitude", table: "Users");
            migrationBuilder.DropColumn(name: "AssignedMapZoom", table: "Users");
        }
    }
}
