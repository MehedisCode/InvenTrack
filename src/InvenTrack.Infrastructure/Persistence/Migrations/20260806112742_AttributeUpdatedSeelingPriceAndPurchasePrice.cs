using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvenTrack.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AttributeUpdatedSeelingPriceAndPurchasePrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CostPrice",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "UnitPrice",
                table: "SaleItems",
                newName: "SellingPrice");

            migrationBuilder.RenameColumn(
                name: "UnitPrice",
                table: "Products",
                newName: "SellingPrice");

            migrationBuilder.AddColumn<decimal>(
                name: "PurchasePrice",
                table: "Products",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444441"),
                column: "PurchasePrice",
                value: 15.00m);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444442"),
                column: "PurchasePrice",
                value: 55.00m);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444443"),
                column: "PurchasePrice",
                value: 3.00m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PurchasePrice",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "SellingPrice",
                table: "SaleItems",
                newName: "UnitPrice");

            migrationBuilder.RenameColumn(
                name: "SellingPrice",
                table: "Products",
                newName: "UnitPrice");

            migrationBuilder.AddColumn<decimal>(
                name: "CostPrice",
                table: "Products",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444441"),
                column: "CostPrice",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444442"),
                column: "CostPrice",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444443"),
                column: "CostPrice",
                value: 0m);
        }
    }
}
