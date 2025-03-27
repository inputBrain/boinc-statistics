using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoincStatistic.Migrations
{
    /// <inheritdoc />
    public partial class addIsScrappingActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsScrappingActive",
                table: "ProjectStats",
                type: "boolean",
                nullable: false,
                defaultValue: false);
            
            migrationBuilder.Sql("UPDATE \"ProjectStats\" SET \"IsScrappingActive\" = true;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsScrappingActive",
                table: "ProjectStats");
        }
    }
}
