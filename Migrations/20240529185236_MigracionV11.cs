using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api_Lucho.Migrations
{
    /// <inheritdoc />
    public partial class MigracionV11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Rol",
                table: "Usuarios",
                newName: "Role");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Role",
                table: "Usuarios",
                newName: "Rol");
        }
    }
}
