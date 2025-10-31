using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoardGameCafe.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTableCustomerReservationEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Customers_CustomerId",
                table: "Reservations");

            // Drop old Table columns
            migrationBuilder.DropColumn(
                name: "Location",
                table: "Tables");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Tables");

            // Rename Capacity to SeatingCapacity
            migrationBuilder.RenameColumn(
                name: "Capacity",
                table: "Tables",
                newName: "SeatingCapacity");

            // Add new Table columns
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Tables",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsWindowSeat",
                table: "Tables",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAccessible",
                table: "Tables",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "HourlyRate",
                table: "Tables",
                type: "TEXT",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            // Rename Customer columns
            migrationBuilder.RenameColumn(
                name: "RegistrationDate",
                table: "Customers",
                newName: "JoinedDate");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "Customers",
                newName: "Phone");

            // Add new Customer column
            migrationBuilder.AddColumn<int>(
                name: "TotalVisits",
                table: "Customers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Table_SeatingCapacity",
                table: "Tables",
                sql: "[SeatingCapacity] >= 2 AND [SeatingCapacity] <= 8");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_ReservationDate",
                table: "Reservations",
                column: "ReservationDate");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_TableId",
                table: "Reservations",
                column: "TableId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Customers_CustomerId",
                table: "Reservations",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Customers_CustomerId",
                table: "Reservations");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Table_SeatingCapacity",
                table: "Tables");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_ReservationDate",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_TableId",
                table: "Reservations");

            // Drop new Table columns
            migrationBuilder.DropColumn(
                name: "HourlyRate",
                table: "Tables");

            migrationBuilder.DropColumn(
                name: "IsAccessible",
                table: "Tables");

            migrationBuilder.DropColumn(
                name: "IsWindowSeat",
                table: "Tables");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Tables");

            // Rename SeatingCapacity back to Capacity
            migrationBuilder.RenameColumn(
                name: "SeatingCapacity",
                table: "Tables",
                newName: "Capacity");

            // Add back old Table columns
            migrationBuilder.AddColumn<int>(
                name: "Location",
                table: "Tables",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Tables",
                type: "INTEGER",
                nullable: false,
                defaultValue: true);

            // Drop new Customer column
            migrationBuilder.DropColumn(
                name: "TotalVisits",
                table: "Customers");

            // Rename Customer columns back
            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "Customers",
                newName: "PhoneNumber");

            migrationBuilder.RenameColumn(
                name: "JoinedDate",
                table: "Customers",
                newName: "RegistrationDate");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Customers_CustomerId",
                table: "Reservations",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
