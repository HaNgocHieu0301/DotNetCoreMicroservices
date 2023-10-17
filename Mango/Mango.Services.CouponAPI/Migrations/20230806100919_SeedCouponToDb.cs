using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mango.Services.CouponAPI.Migrations
{
    public partial class SeedCouponToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Coupons",
                columns: new[] { "CouponId", "CounponCode", "DiscountAmount", "MinAmount" },
                values: new object[] { 1, "100FF", 10.0, 10 });

            migrationBuilder.InsertData(
                table: "Coupons",
                columns: new[] { "CouponId", "CounponCode", "DiscountAmount", "MinAmount" },
                values: new object[] { 2, "200FF", 20.0, 40 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Coupons",
                keyColumn: "CouponId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Coupons",
                keyColumn: "CouponId",
                keyValue: 2);
        }
    }
}
