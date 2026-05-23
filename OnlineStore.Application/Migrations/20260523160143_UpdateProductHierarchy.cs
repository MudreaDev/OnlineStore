using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineStore.Application.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductHierarchy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WarrantyMonths",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "VehicleProduct_Model",
                table: "Products",
                newName: "ClothingProduct_Size");

            migrationBuilder.RenameColumn(
                name: "Model",
                table: "Products",
                newName: "ClothingProduct_Material");

            migrationBuilder.RenameColumn(
                name: "Make",
                table: "Products",
                newName: "ClothingProduct_AvailableSizes");

            migrationBuilder.RenameColumn(
                name: "FuelType",
                table: "Products",
                newName: "AccessoryProduct_Material");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ClothingProduct_Size",
                table: "Products",
                newName: "VehicleProduct_Model");

            migrationBuilder.RenameColumn(
                name: "ClothingProduct_Material",
                table: "Products",
                newName: "Model");

            migrationBuilder.RenameColumn(
                name: "ClothingProduct_AvailableSizes",
                table: "Products",
                newName: "Make");

            migrationBuilder.RenameColumn(
                name: "AccessoryProduct_Material",
                table: "Products",
                newName: "FuelType");

            migrationBuilder.AddColumn<int>(
                name: "WarrantyMonths",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "Products",
                type: "int",
                nullable: true);
        }
    }
}
