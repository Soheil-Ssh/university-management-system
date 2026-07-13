using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Faculty.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDeanToFacultyTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DeanProfessorId",
                table: "Faculties",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Faculties_DeanProfessorId",
                table: "Faculties",
                column: "DeanProfessorId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Faculties_Professors_DeanProfessorId",
                table: "Faculties",
                column: "DeanProfessorId",
                principalTable: "Professors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Faculties_Professors_DeanProfessorId",
                table: "Faculties");

            migrationBuilder.DropIndex(
                name: "IX_Faculties_DeanProfessorId",
                table: "Faculties");

            migrationBuilder.DropColumn(
                name: "DeanProfessorId",
                table: "Faculties");
        }
    }
}
