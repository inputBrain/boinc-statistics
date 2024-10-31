using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoincStatistic.Migrations
{
    /// <inheritdoc />
    public partial class test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BoincStats_ProjectId",
                table: "BoincStats");

            migrationBuilder.CreateIndex(
                name: "IX_BoincStats_ProjectId",
                table: "BoincStats",
                column: "ProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BoincStats_ProjectId",
                table: "BoincStats");

            migrationBuilder.CreateIndex(
                name: "IX_BoincStats_ProjectId",
                table: "BoincStats",
                column: "ProjectId",
                unique: true);
        }
    }
}
