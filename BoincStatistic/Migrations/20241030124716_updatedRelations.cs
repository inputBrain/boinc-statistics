using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoincStatistic.Migrations
{
    /// <inheritdoc />
    public partial class updatedRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "BoincStats",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BoincStats_ProjectId",
                table: "BoincStats",
                column: "ProjectId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BoincStats_ProjectStats_ProjectId",
                table: "BoincStats",
                column: "ProjectId",
                principalTable: "ProjectStats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoincStats_ProjectStats_ProjectId",
                table: "BoincStats");

            migrationBuilder.DropIndex(
                name: "IX_BoincStats_ProjectId",
                table: "BoincStats");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "BoincStats");
        }
    }
}
