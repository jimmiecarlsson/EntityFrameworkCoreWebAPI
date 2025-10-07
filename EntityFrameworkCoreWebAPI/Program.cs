using EntityFrameworkCoreWebAPI.Data;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Registrera Services
builder.Services.AddOpenApi();
builder.Services.AddDbContext<TodoDb>(options => options.UseSqlite("Data Source=todo.db"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); // Scalar UI
}

app.MapGet("/todos", async (TodoDb db) => await db.TodoItems.ToListAsync());
app.MapPost("/todos", async (TodoDb db, TodoItem item) =>
{
    db.TodoItems.Add(item);
    await db.SaveChangesAsync();
    return Results.Created($"/todos/{item.Id}", item);
});


app.Run();

