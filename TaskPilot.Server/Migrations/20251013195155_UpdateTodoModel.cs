using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskPilot.Server.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTodoModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalPomoTotalPomodoroSessions",
                table: "Stats");

            migrationBuilder.AddColumn<int>(
                name: "TotalPomodoroSessions",
                table: "Stats",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalPomodoroSessions",
                table: "Stats");

            migrationBuilder.AddColumn<TimeOnly>(
                name: "TotalPomoTotalPomodoroSessions",
                table: "Stats",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));
        }
    }
}
