using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ms_inventary.Migrations
{
    /// <inheritdoc />
    public partial class newmodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Supplier_Company_CompanyId",
                table: "Supplier");

            migrationBuilder.DropIndex(
                name: "IX_Supplier_CompanyId",
                table: "Supplier");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductHasSupplier",
                table: "ProductHasSupplier");

            migrationBuilder.DropIndex(
                name: "IX_ProductHasSupplier_ProductId",
                table: "ProductHasSupplier");

            migrationBuilder.AlterColumn<int>(
                name: "SupplierId",
                table: "Supplier",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<double>(
                name: "CostPrice",
                table: "ProductHasSupplier",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0)
                .Annotation("Relational:ColumnOrder", 4);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "ProductHasSupplier",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Relational:ColumnOrder", 5);

            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "ProductHasSupplier",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Relational:ColumnOrder", 6);

            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "ProductHasSupplier",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Relational:ColumnOrder", 7);

            migrationBuilder.AddColumn<bool>(
                name: "Enabled",
                table: "Branch",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductHasSupplier",
                table: "ProductHasSupplier",
                columns: new[] { "ProductId", "SupplierId", "PurchaseDate", "CostPrice", "Quantity", "OrderId", "BranchId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductHasSupplier_SupplierId",
                table: "ProductHasSupplier",
                column: "SupplierId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductHasSupplier",
                table: "ProductHasSupplier");

            migrationBuilder.DropIndex(
                name: "IX_ProductHasSupplier_SupplierId",
                table: "ProductHasSupplier");

            migrationBuilder.DropColumn(
                name: "CostPrice",
                table: "ProductHasSupplier");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "ProductHasSupplier");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "ProductHasSupplier");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "ProductHasSupplier");

            migrationBuilder.DropColumn(
                name: "Enabled",
                table: "Branch");

            migrationBuilder.AlterColumn<int>(
                name: "SupplierId",
                table: "Supplier",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductHasSupplier",
                table: "ProductHasSupplier",
                columns: new[] { "SupplierId", "ProductId" });

            migrationBuilder.CreateIndex(
                name: "IX_Supplier_CompanyId",
                table: "Supplier",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductHasSupplier_ProductId",
                table: "ProductHasSupplier",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Supplier_Company_CompanyId",
                table: "Supplier",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "CompanyId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}