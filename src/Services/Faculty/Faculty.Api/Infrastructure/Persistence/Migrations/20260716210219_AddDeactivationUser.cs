using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Faculty.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDeactivationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdentityDeactivationFailureReason",
                table: "Professors",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdentityDeactivationStatus",
                table: "Professors",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ProfessorIdentityDeactivationStates",
                columns: table => new
                {
                    CorrelationId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentState = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ProfessorId = table.Column<Guid>(type: "uuid", nullable: false),
                    IdentityUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FailureReason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfessorIdentityDeactivationStates", x => x.CorrelationId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProfessorIdentityDeactivationStates_IdentityUserId",
                table: "ProfessorIdentityDeactivationStates",
                column: "IdentityUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfessorIdentityDeactivationStates_ProfessorId",
                table: "ProfessorIdentityDeactivationStates",
                column: "ProfessorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProfessorIdentityDeactivationStates");

            migrationBuilder.DropColumn(
                name: "IdentityDeactivationFailureReason",
                table: "Professors");

            migrationBuilder.DropColumn(
                name: "IdentityDeactivationStatus",
                table: "Professors");
        }
    }
}
