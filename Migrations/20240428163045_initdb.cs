using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ms_inventary.Migrations
{
    /// <inheritdoc />
    public partial class initdb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "Company",
                columns: table => new
                {
                    CompanyId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NIT = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    businessArea = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    employeeNumber = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Company", x => x.CompanyId);
                });

            migrationBuilder.CreateTable(
                name: "ServiceType",
                columns: table => new
                {
                    ServiceTypeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceType", x => x.ServiceTypeId);
                });

            migrationBuilder.CreateTable(
                name: "SupplierType",
                columns: table => new
                {
                    SupplierTypeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierType", x => x.SupplierTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    Description = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    CostPrice = table.Column<double>(type: "double precision", nullable: false),
                    SalePrice = table.Column<double>(type: "double precision", nullable: false),
                    CategoryId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_Product_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Branch",
                columns: table => new
                {
                    BranchId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    Address = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    CompanyId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branch", x => x.BranchId);
                    table.ForeignKey(
                        name: "FK_Branch_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Supplier",
                columns: table => new
                {
                    SupplierId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    Address = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    Phone = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    Email = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    UrlPage = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    SupplierTypeId = table.Column<int>(type: "integer", nullable: false),
                    ServiceTypeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supplier", x => x.SupplierId);
                    table.ForeignKey(
                        name: "FK_Supplier_ServiceType_ServiceTypeId",
                        column: x => x.ServiceTypeId,
                        principalTable: "ServiceType",
                        principalColumn: "ServiceTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Supplier_SupplierType_SupplierTypeId",
                        column: x => x.SupplierTypeId,
                        principalTable: "SupplierType",
                        principalColumn: "SupplierTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BranchHasProduct",
                columns: table => new
                {
                    BranchId = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    Discount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BranchHasProduct", x => new { x.BranchId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_BranchHasProduct_Branch_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branch",
                        principalColumn: "BranchId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BranchHasProduct_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductHasSupplier",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    SupplierId = table.Column<int>(type: "integer", nullable: false),
                    PurchaseDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductHasSupplier", x => new { x.SupplierId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_ProductHasSupplier_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductHasSupplier_Supplier_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Supplier",
                        principalColumn: "SupplierId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Branch_CompanyId",
                table: "Branch",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_BranchHasProduct_ProductId",
                table: "BranchHasProduct",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_CategoryId",
                table: "Product",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductHasSupplier_ProductId",
                table: "ProductHasSupplier",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Supplier_ServiceTypeId",
                table: "Supplier",
                column: "ServiceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Supplier_SupplierTypeId",
                table: "Supplier",
                column: "SupplierTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BranchHasProduct");

            migrationBuilder.DropTable(
                name: "ProductHasSupplier");

            migrationBuilder.DropTable(
                name: "Branch");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "Supplier");

            migrationBuilder.DropTable(
                name: "Company");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "ServiceType");

            migrationBuilder.DropTable(
                name: "SupplierType");
        }
    }
}
