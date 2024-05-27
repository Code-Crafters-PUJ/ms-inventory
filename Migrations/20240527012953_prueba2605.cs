using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ms_inventary.Migrations
{
    /// <inheritdoc />
    public partial class prueba2605 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Enabled",
                table: "Branch");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Enabled",
                table: "Branch",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
