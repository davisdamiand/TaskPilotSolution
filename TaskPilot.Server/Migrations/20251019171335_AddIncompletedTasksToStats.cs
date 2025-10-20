using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskPilot.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddIncompletedTasksToStats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalInCompletedTasks",
                table: "Stats",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalInCompletedTasks",
                table: "Stats");
        }
    }
}
