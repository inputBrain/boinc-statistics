using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoincStatistic.Migrations
{
    /// <inheritdoc />
    public partial class addIsCreditDayZero : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCreditDayZero",
                table: "ProjectStats",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCreditDayZero",
                table: "ProjectStats");
        }
    }
}
