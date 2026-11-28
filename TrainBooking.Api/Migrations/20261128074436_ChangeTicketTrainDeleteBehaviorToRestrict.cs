using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainBooking.Api.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTicketTrainDeleteBehaviorToRestrict : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Trains_TrainId",
                table: "Tickets");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Trains_TrainId",
                table: "Tickets",
                column: "TrainId",
                principalTable: "Trains",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Trains_TrainId",
                table: "Tickets");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Trains_TrainId",
                table: "Tickets",
                column: "TrainId",
                principalTable: "Trains",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
