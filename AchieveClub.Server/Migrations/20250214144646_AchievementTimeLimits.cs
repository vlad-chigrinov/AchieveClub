using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AchieveClub.Server.Migrations
{
    /// <inheritdoc />
    public partial class AchievementTimeLimits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TimeLimitInDays",
                table: "Achievements",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeLimitInDays",
                table: "Achievements");
        }
    }
}
