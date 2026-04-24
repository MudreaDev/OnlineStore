using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineStore.Application.Migrations
{
    /// <inheritdoc />
    public partial class FixVehicleProductSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FuelType",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Make",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VehicleProduct_Model",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FuelType",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Make",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "VehicleProduct_Model",
                table: "Products");
        }
    }
}
