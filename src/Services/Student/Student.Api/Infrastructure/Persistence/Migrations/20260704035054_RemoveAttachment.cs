using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Student.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAttachment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdmissionAttachments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdmissionAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AdmissionRequestId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FileId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdmissionAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdmissionAttachments_AdmissionRequests_AdmissionRequestId",
                        column: x => x.AdmissionRequestId,
                        principalTable: "AdmissionRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionAttachments_AdmissionRequestId",
                table: "AdmissionAttachments",
                column: "AdmissionRequestId");
        }
    }
}
