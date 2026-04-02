using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineStore.Application.Migrations
{
    /// <inheritdoc />
    public partial class AddProductVariantsAndSubscribers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SubscriberEmails",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubscriberEmails",
                table: "Products");
        }
    }
}
