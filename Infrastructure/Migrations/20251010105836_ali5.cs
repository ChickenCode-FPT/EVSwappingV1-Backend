using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ali5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReservationAllocation_Batteries_BatteryId",
                table: "ReservationAllocation");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservationAllocation_Reservations_ReservationId",
                table: "ReservationAllocation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReservationAllocation",
                table: "ReservationAllocation");

            migrationBuilder.RenameTable(
                name: "ReservationAllocation",
                newName: "ReservationAllocations");

            migrationBuilder.RenameIndex(
                name: "IX_ReservationAllocation_ReservationId",
                table: "ReservationAllocations",
                newName: "IX_ReservationAllocations_ReservationId");

            migrationBuilder.RenameIndex(
                name: "IX_ReservationAllocation_BatteryId_Status",
                table: "ReservationAllocations",
                newName: "IX_ReservationAllocations_BatteryId_Status");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReservationAllocations",
                table: "ReservationAllocations",
                column: "ReservationAllocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReservationAllocations_Batteries_BatteryId",
                table: "ReservationAllocations",
                column: "BatteryId",
                principalTable: "Batteries",
                principalColumn: "BatteryId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReservationAllocations_Reservations_ReservationId",
                table: "ReservationAllocations",
                column: "ReservationId",
                principalTable: "Reservations",
                principalColumn: "ReservationId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReservationAllocations_Batteries_BatteryId",
                table: "ReservationAllocations");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservationAllocations_Reservations_ReservationId",
                table: "ReservationAllocations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReservationAllocations",
                table: "ReservationAllocations");

            migrationBuilder.RenameTable(
                name: "ReservationAllocations",
                newName: "ReservationAllocation");

            migrationBuilder.RenameIndex(
                name: "IX_ReservationAllocations_ReservationId",
                table: "ReservationAllocation",
                newName: "IX_ReservationAllocation_ReservationId");

            migrationBuilder.RenameIndex(
                name: "IX_ReservationAllocations_BatteryId_Status",
                table: "ReservationAllocation",
                newName: "IX_ReservationAllocation_BatteryId_Status");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReservationAllocation",
                table: "ReservationAllocation",
                column: "ReservationAllocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReservationAllocation_Batteries_BatteryId",
                table: "ReservationAllocation",
                column: "BatteryId",
                principalTable: "Batteries",
                principalColumn: "BatteryId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReservationAllocation_Reservations_ReservationId",
                table: "ReservationAllocation",
                column: "ReservationId",
                principalTable: "Reservations",
                principalColumn: "ReservationId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
