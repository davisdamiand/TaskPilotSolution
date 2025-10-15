using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskPilot.Server.Migrations
{
    /// <inheritdoc />
    public partial class ChangedTimeSpentToMinutes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeSpent",
                table: "Todos");

            migrationBuilder.AddColumn<int>(
                name: "TimeSpentMinutes",
                table: "Todos",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeSpentMinutes",
                table: "Todos");

            migrationBuilder.AddColumn<TimeOnly>(
                name: "TimeSpent",
                table: "Todos",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));
        }
    }
}
