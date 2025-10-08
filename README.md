# Minimal Web API med SQLite, Entity Framework Core och Scalar (OpenAPI)

Denna guide visar hur du skapar ett **Minimal Web API** i .NET med **SQLite** och **Entity Framework Core**, samt hur du exponerar och testar API:et med **OpenAPI** via **Scalar** — ett modernt alternativ till Swagger UI.

---

## Förutsättningar

- .NET 6 eller senare installerad. Jag körde .Net 9  
- En utvecklingsmiljö som kan köra .NET-projekt (Visual Studio, VS Code eller CLI)  
- Grundläggande kunskap om C#, Web API och Entity Framework Core  

---

## Nödvändiga NuGet-paket

Installera följande paket:

```bash
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.Relational
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package DotNetEnv
dotnet add package Scalar.AspNetCore
```

> Dokumentation:  
> - EF Core installation: [learn.microsoft.com/ef/core/get-started/overview/install](https://learn.microsoft.com/en-us/ef/core/get-started/overview/install)  
> - Modul: [Build a web API with a database in ASP.NET Core](https://learn.microsoft.com/en-us/training/modules/build-web-api-minimal-database/)

---

## Att göra-lista (för att få allt att fungera)

### 1. Skapa projekt
```bash
dotnet new webapi -n MinimalApiWithScalar
cd MinimalApiWithScalar
```

### 2. Lägg till EF Core-paket, Scalar och DotNetEnv
Se ovanstående `dotnet add package`-kommandon.

---

### 3. Skapa datamodell och DbContext
Exempel:
```csharp
using Microsoft.EntityFrameworkCore;

namespace MinimalApiWithScalar.Data;

public class TodoItem
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public bool IsComplete { get; set; }
}

public class TodoDb : DbContext
{
    public TodoDb(DbContextOptions<TodoDb> options) : base(options) { }
    public DbSet<TodoItem> Todos => Set<TodoItem>();
}
```

---

### 4. Registrera tjänster i `Program.cs`
```csharp
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using DotNetEnv; // <-- Lägg till för att ha DB_CONNECTION i .env

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

Env.Load(); // <-- Lägg till för att ha DB_CONNECTION i .env

var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION"); // <-- Lägg till för att ha DB_CONNECTION i .env

builder.Services.AddDbContext<TodoDb>(options =>
    options.UseSqlite("connectionString")); // <-- Lägg till för att ha DB_CONNECTION i .env

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapGet("/todos", async (TodoDb db) => await db.Todos.ToListAsync());
app.MapPost("/todos", async (TodoDb db, TodoItem item) =>
{
    db.Todos.Add(item);
    await db.SaveChangesAsync();
    return Results.Created($"/todos/{item.Id}", item);
});

app.Run();
```
---

### 4.b Skapa en .env fil ir projekt-rooten `.env`
Med innehållet och namnet du vill ha på databasen. Och göm den med .gitignore

```
DB_CONNECTION="Data Source=mindatabas.db"
```
---

### 5. Kontrollera CLI och installera `dotnet-ef`
Innan du kör migrationer, kontrollera att verktyget **Entity Framework CLI** är installerat.

```bash
dotnet --version
```
Om detta fungerar, installera eller uppdatera `dotnet-ef`:

```bash
dotnet tool install --global dotnet-ef
dotnet tool update --global dotnet-ef
dotnet ef --version
```
Om `dotnet ef` inte känns igen, starta om PowerShell/VS Code och kontrollera att följande sökväg finns i PATH:
```
%USERPROFILE%\.dotnet\tools
```

---

### 6. Skapa och migrera databas
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

---

### 7. Kör projektet
```bash
dotnet run
```

Öppna sedan i webbläsaren:  
👉 **`https://localhost:5001/scalar/v1`**

Där hittar du Scalar-gränssnittet med din OpenAPI-specifikation (ersätter Swagger).

---

## Scalar — kort om fördelarna

Scalar är ett snyggt, modernt alternativ till Swagger UI.  
Fördelar:
- Snabb rendering (ingen tung JS som Swagger UI)  
- Mörkt och ljust tema  
- Bättre kodhighlighting  
- Genererad dokumentation direkt från dina Minimal API-endpoints  

Mer info: [https://scalar.com/openapi](https://scalar.com/openapi)

---

## Tips

- För produktion: avaktivera `app.MapScalarApiReference()` utanför `Development`-miljö.  
- Kontrollera att SQLite-filen (`todo.db`) skapas i projektets rotmapp.  
- Använd `dotnet watch run` under utveckling för snabbare feedback.  

---

## Sammanfattning

✅ Minimal API  
✅ SQLite via EF Core  
✅ OpenAPI-dokumentation  
✅ Scalar istället för Swagger UI  
✅ CLI-version & dotnet-ef-kontroll inlagd
