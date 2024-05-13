using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ms_inventary.Migrations
{
    /// <inheritdoc />
    public partial class supplier_cambio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Company_Branch_BranchId",
                table: "Company");

            migrationBuilder.DropIndex(
                name: "IX_Company_BranchId",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "Company");

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Supplier",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Supplier_CompanyId",
                table: "Supplier",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Supplier_Company_CompanyId",
                table: "Supplier",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "CompanyId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Supplier_Company_CompanyId",
                table: "Supplier");

            migrationBuilder.DropIndex(
                name: "IX_Supplier_CompanyId",
                table: "Supplier");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Supplier");

            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "Company",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Company_BranchId",
                table: "Company",
                column: "BranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_Company_Branch_BranchId",
                table: "Company",
                column: "BranchId",
                principalTable: "Branch",
                principalColumn: "BranchId");
        }
    }
}
