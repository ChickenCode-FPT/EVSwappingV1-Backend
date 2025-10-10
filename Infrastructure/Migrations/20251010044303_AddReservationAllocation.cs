using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReservationAllocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReservationId",
                table: "StationInventory",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ReservationAllocation",
                columns: table => new
                {
                    ReservationAllocationId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReservationId = table.Column<int>(type: "int", nullable: false),
                    BatteryId = table.Column<int>(type: "int", nullable: false),
                    AllocatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoldUntil = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationAllocation", x => x.ReservationAllocationId);
                    table.ForeignKey(
                        name: "FK_ReservationAllocation_Batteries_BatteryId",
                        column: x => x.BatteryId,
                        principalTable: "Batteries",
                        principalColumn: "BatteryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReservationAllocation_Reservations_ReservationId",
                        column: x => x.ReservationId,
                        principalTable: "Reservations",
                        principalColumn: "ReservationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StationInventory_ReservationId",
                table: "StationInventory",
                column: "ReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationAllocation_BatteryId_Status",
                table: "ReservationAllocation",
                columns: new[] { "BatteryId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ReservationAllocation_ReservationId",
                table: "ReservationAllocation",
                column: "ReservationId");

            migrationBuilder.AddForeignKey(
                name: "FK_StationInventory_Reservations_ReservationId",
                table: "StationInventory",
                column: "ReservationId",
                principalTable: "Reservations",
                principalColumn: "ReservationId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StationInventory_Reservations_ReservationId",
                table: "StationInventory");

            migrationBuilder.DropTable(
                name: "ReservationAllocation");

            migrationBuilder.DropIndex(
                name: "IX_StationInventory_ReservationId",
                table: "StationInventory");

            migrationBuilder.DropColumn(
                name: "ReservationId",
                table: "StationInventory");
        }
    }
}
