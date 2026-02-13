using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DistributedSystemAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    street_address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "Video_Games",
                columns: table => new
                {
                    game_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    publisher = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    year_published = table.Column<int>(type: "int", nullable: false),
                    system = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    previous_owners_count = table.Column<int>(type: "int", nullable: true),
                    condition = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Video_Games", x => x.game_id);
                    table.ForeignKey(
                        name: "FK_Video_Games_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Trade_Offers",
                columns: table => new
                {
                    trade_offer_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    from_user_id = table.Column<int>(type: "int", nullable: false),
                    to_user_id = table.Column<int>(type: "int", nullable: false),
                    requested_game_id = table.Column<int>(type: "int", nullable: false),
                    offered_game_id = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    responded_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trade_Offers", x => x.trade_offer_id);
                    table.ForeignKey(
                        name: "FK_Trade_Offers_Users_from_user_id",
                        column: x => x.from_user_id,
                        principalTable: "Users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Trade_Offers_Users_to_user_id",
                        column: x => x.to_user_id,
                        principalTable: "Users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Trade_Offers_Video_Games_offered_game_id",
                        column: x => x.offered_game_id,
                        principalTable: "Video_Games",
                        principalColumn: "game_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Trade_Offers_Video_Games_requested_game_id",
                        column: x => x.requested_game_id,
                        principalTable: "Video_Games",
                        principalColumn: "game_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Trade_Offers_from_user_id",
                table: "Trade_Offers",
                column: "from_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Trade_Offers_offered_game_id",
                table: "Trade_Offers",
                column: "offered_game_id");

            migrationBuilder.CreateIndex(
                name: "IX_Trade_Offers_requested_game_id",
                table: "Trade_Offers",
                column: "requested_game_id");

            migrationBuilder.CreateIndex(
                name: "IX_Trade_Offers_to_user_id",
                table: "Trade_Offers",
                column: "to_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Users_email",
                table: "Users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Video_Games_user_id",
                table: "Video_Games",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Trade_Offers");

            migrationBuilder.DropTable(
                name: "Video_Games");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
