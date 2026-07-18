using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Faculty.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartmentProfessorAssignmentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DepartmentProfessorAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProfessorId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UnassignedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentProfessorAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DepartmentProfessorAssignments_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DepartmentProfessorAssignments_Professors_ProfessorId",
                        column: x => x.ProfessorId,
                        principalTable: "Professors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentProfessorAssignments_DepartmentId",
                table: "DepartmentProfessorAssignments",
                column: "DepartmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentProfessorAssignments_ProfessorId",
                table: "DepartmentProfessorAssignments",
                column: "ProfessorId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_DepartmentProfessorAssignments_ActiveAssignment",
                table: "DepartmentProfessorAssignments",
                columns: new[] { "DepartmentId", "ProfessorId" },
                unique: true,
                filter: "\"UnassignedAt\" IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DepartmentProfessorAssignments");
        }
    }
}
