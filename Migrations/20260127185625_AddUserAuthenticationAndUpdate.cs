using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinemaBooking.Migrations
{
    /// <inheritdoc />
    public partial class AddUserAuthenticationAndUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Seat_SeatId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_showTimes_ShowTimeId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Seat_Cinemas_CinemaId",
                table: "Seat");

            migrationBuilder.DropForeignKey(
                name: "FK_showTimes_Cinemas_CinemaId",
                table: "showTimes");

            migrationBuilder.DropForeignKey(
                name: "FK_showTimes_Movies_MovieId",
                table: "showTimes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_showTimes",
                table: "showTimes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Seat",
                table: "Seat");

            migrationBuilder.RenameTable(
                name: "showTimes",
                newName: "ShowTimes");

            migrationBuilder.RenameTable(
                name: "Seat",
                newName: "Seats");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "users",
                newName: "PasswordHash");

            migrationBuilder.RenameIndex(
                name: "IX_showTimes_MovieId",
                table: "ShowTimes",
                newName: "IX_ShowTimes_MovieId");

            migrationBuilder.RenameIndex(
                name: "IX_showTimes_CinemaId",
                table: "ShowTimes",
                newName: "IX_ShowTimes_CinemaId");

            migrationBuilder.RenameColumn(
                name: "CinemaId",
                table: "Seats",
                newName: "ShowTimeId");

            migrationBuilder.RenameIndex(
                name: "IX_Seat_CinemaId",
                table: "Seats",
                newName: "IX_Seats_ShowTimeId");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "users",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "users",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RoomNumber",
                table: "ShowTimes",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "StartTime",
                table: "ShowTimes",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Cinemas",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Cinemas",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Seats",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "SeatType",
                table: "Seats",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShowTimes",
                table: "ShowTimes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Seats",
                table: "Seats",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TicketCode = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    IssuedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Status = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tickets_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_BookingId",
                table: "Tickets",
                column: "BookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Seats_SeatId",
                table: "Bookings",
                column: "SeatId",
                principalTable: "Seats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_ShowTimes_ShowTimeId",
                table: "Bookings",
                column: "ShowTimeId",
                principalTable: "ShowTimes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Seats_ShowTimes_ShowTimeId",
                table: "Seats",
                column: "ShowTimeId",
                principalTable: "ShowTimes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShowTimes_Cinemas_CinemaId",
                table: "ShowTimes",
                column: "CinemaId",
                principalTable: "Cinemas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShowTimes_Movies_MovieId",
                table: "ShowTimes",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Seats_SeatId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_ShowTimes_ShowTimeId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Seats_ShowTimes_ShowTimeId",
                table: "Seats");

            migrationBuilder.DropForeignKey(
                name: "FK_ShowTimes_Cinemas_CinemaId",
                table: "ShowTimes");

            migrationBuilder.DropForeignKey(
                name: "FK_ShowTimes_Movies_MovieId",
                table: "ShowTimes");

            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ShowTimes",
                table: "ShowTimes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Seats",
                table: "Seats");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "users");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "users");

            migrationBuilder.DropColumn(
                name: "RoomNumber",
                table: "ShowTimes");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "ShowTimes");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Cinemas");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Cinemas");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Seats");

            migrationBuilder.DropColumn(
                name: "SeatType",
                table: "Seats");

            migrationBuilder.RenameTable(
                name: "ShowTimes",
                newName: "showTimes");

            migrationBuilder.RenameTable(
                name: "Seats",
                newName: "Seat");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "users",
                newName: "Password");

            migrationBuilder.RenameIndex(
                name: "IX_ShowTimes_MovieId",
                table: "showTimes",
                newName: "IX_showTimes_MovieId");

            migrationBuilder.RenameIndex(
                name: "IX_ShowTimes_CinemaId",
                table: "showTimes",
                newName: "IX_showTimes_CinemaId");

            migrationBuilder.RenameColumn(
                name: "ShowTimeId",
                table: "Seat",
                newName: "CinemaId");

            migrationBuilder.RenameIndex(
                name: "IX_Seats_ShowTimeId",
                table: "Seat",
                newName: "IX_Seat_CinemaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_showTimes",
                table: "showTimes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Seat",
                table: "Seat",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Seat_SeatId",
                table: "Bookings",
                column: "SeatId",
                principalTable: "Seat",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_showTimes_ShowTimeId",
                table: "Bookings",
                column: "ShowTimeId",
                principalTable: "showTimes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Seat_Cinemas_CinemaId",
                table: "Seat",
                column: "CinemaId",
                principalTable: "Cinemas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_showTimes_Cinemas_CinemaId",
                table: "showTimes",
                column: "CinemaId",
                principalTable: "Cinemas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_showTimes_Movies_MovieId",
                table: "showTimes",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
