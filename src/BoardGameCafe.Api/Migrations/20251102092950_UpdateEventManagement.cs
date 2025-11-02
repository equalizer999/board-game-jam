using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoardGameCafe.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEventManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Event_Capacity",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "CurrentParticipants",
                table: "Events");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Events",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventRegistrations_EventId_CustomerId",
                table: "EventRegistrations",
                columns: new[] { "EventId", "CustomerId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EventRegistrations_EventId_CustomerId",
                table: "EventRegistrations");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Events");

            migrationBuilder.AddColumn<int>(
                name: "CurrentParticipants",
                table: "Events",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Event_Capacity",
                table: "Events",
                sql: "[CurrentParticipants] <= [MaxParticipants]");
        }
    }
}
