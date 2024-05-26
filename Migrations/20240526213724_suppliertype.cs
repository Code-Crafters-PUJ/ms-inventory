using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ms_inventary.Migrations
{
    /// <inheritdoc />
   public partial class Suppliertype : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Eliminar datos existentes en SupplierType
        migrationBuilder.Sql("DELETE FROM \"SupplierType\"");

        // Insertar nuevos datos en SupplierType
        migrationBuilder.InsertData(
            table: "SupplierType",
            columns: new[] { "SupplierTypeId", "Name" },
            values: new object[,]
            {
                { 1, "persona" },
                { 2, "empresa" },
                { 3, "organizacion" },
                { 4, "fundacion" }
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Eliminar los datos insertados
        migrationBuilder.Sql("DELETE FROM \"SupplierType\" WHERE \"SupplierTypeId\" IN (1, 2, 3, 4)");
    }
}

}
