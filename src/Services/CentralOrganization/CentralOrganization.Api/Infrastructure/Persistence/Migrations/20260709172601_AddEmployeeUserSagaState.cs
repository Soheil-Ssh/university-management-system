using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CentralOrganization.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeUserSagaState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmployeeIdentityProvisioningSagas",
                columns: table => new
                {
                    CorrelationId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    NationalCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    MobileNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CurrentState = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IdentityUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    FailureReason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeIdentityProvisioningSagas", x => x.CorrelationId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeIdentityProvisioningSagas_EmployeeId",
                table: "EmployeeIdentityProvisioningSagas",
                column: "EmployeeId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeIdentityProvisioningSagas");
        }
    }
}
