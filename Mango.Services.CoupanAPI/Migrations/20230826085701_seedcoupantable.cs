using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mango.Services.CoupanAPI.Migrations
{
    public partial class seedcoupantable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Coupons",
                columns: new[] { "CoupanId", "CoupanCode", "DiscountAmount", "MinAmount" },
                values: new object[] { 1, "10OFF", 10.0, 20 });

            migrationBuilder.InsertData(
                table: "Coupons",
                columns: new[] { "CoupanId", "CoupanCode", "DiscountAmount", "MinAmount" },
                values: new object[] { 2, "20OFF", 20.0, 40 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Coupons",
                keyColumn: "CoupanId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Coupons",
                keyColumn: "CoupanId",
                keyValue: 2);
        }
    }
}
