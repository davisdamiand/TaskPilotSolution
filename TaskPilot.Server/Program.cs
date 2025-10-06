using Microsoft.EntityFrameworkCore;
using TaskPilot.Server.Data;
using TaskPilot.Server.Interfaces;
using TaskPilot.Server.Services;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Register context
builder.Services.AddDbContext<TaskPilotContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IStudentService, StudentService>();


builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
