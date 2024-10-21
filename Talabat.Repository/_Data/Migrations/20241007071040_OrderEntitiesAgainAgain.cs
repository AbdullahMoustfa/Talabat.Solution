using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Talabat.Repository._Data.Migrations
{
    public partial class OrderEntitiesAgainAgain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DeliveryItem",
                table: "DeliveryMethods",
                newName: "DeliveryTime");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DeliveryTime",
                table: "DeliveryMethods",
                newName: "DeliveryItem");
        }
    }
}
