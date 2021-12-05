using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModMail.Migrations
{
    public partial class Threadsv1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ModMailCategory",
                table: "Guilds",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ModMailLogChannel",
                table: "Guilds",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "ModMailThreadEntity",
                columns: table => new
                {
                    Recepient = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Channel = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    GuildId = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModMailThreadEntity", x => x.Recepient);
                    table.ForeignKey(
                        name: "FK_ModMailThreadEntity_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "GuildId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ModMailThreadEntity_GuildId",
                table: "ModMailThreadEntity",
                column: "GuildId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModMailThreadEntity");

            migrationBuilder.DropColumn(
                name: "ModMailCategory",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "ModMailLogChannel",
                table: "Guilds");
        }
    }
}
