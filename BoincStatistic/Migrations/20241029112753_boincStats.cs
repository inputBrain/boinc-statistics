using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BoincStatistic.Migrations
{
    /// <inheritdoc />
    public partial class boincStats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BoincStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Rank = table.Column<string>(type: "text", nullable: true),
                    CountryName = table.Column<string>(type: "text", nullable: true),
                    TotalCredit = table.Column<string>(type: "text", nullable: true),
                    CreditDay = table.Column<string>(type: "text", nullable: true),
                    CreditWeek = table.Column<string>(type: "text", nullable: true),
                    CreditMonth = table.Column<string>(type: "text", nullable: true),
                    CreditAvarage = table.Column<string>(type: "text", nullable: true),
                    CreditUser = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoincStats", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BoincStats");
        }
    }
}
