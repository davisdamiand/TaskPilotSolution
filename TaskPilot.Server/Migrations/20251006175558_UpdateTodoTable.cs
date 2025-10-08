using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskPilot.Server.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTodoTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "PrioritySelection",
                table: "Todos",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrioritySelection",
                table: "Todos");
        }
    }
}
