using System;

using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMoleculesAndMedicineMolecules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Molecules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Molecules", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MedicineMolecules",
                columns: table => new
                {
                    MedicineId = table.Column<int>(type: "int", nullable: false),
                    MoleculeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicineMolecules", x => new { x.MedicineId, x.MoleculeId });
                    table.ForeignKey(
                        name: "FK_MedicineMolecules_Medicines_MedicineId",
                        column: x => x.MedicineId,
                        principalTable: "Medicines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MedicineMolecules_Molecules_MoleculeId",
                        column: x => x.MoleculeId,
                        principalTable: "Molecules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_MedicineMolecules_MoleculeId",
                table: "MedicineMolecules",
                column: "MoleculeId");

            migrationBuilder.CreateIndex(
                name: "IX_Molecules_Name",
                table: "Molecules",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MedicineMolecules");

            migrationBuilder.DropTable(
                name: "Molecules");
        }
    }
}
