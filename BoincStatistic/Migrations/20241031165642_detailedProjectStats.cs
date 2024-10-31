using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BoincStatistic.Migrations
{
    /// <inheritdoc />
    public partial class detailedProjectStats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DetailedProjectStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_DetailedProjectStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetailedProjectStats_ProjectStats_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "ProjectStats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DetailedProjectStats_ProjectId",
                table: "DetailedProjectStats",
                column: "ProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetailedProjectStats");
        }
    }
}
