using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CentralOrganization.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentityProvisioningStatusToEmplyeeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdentityProvisioningStatus",
                table: "Employees",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdentityProvisioningStatus",
                table: "Employees");
        }
    }
}
