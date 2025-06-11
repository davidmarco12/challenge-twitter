using Infrastructure;
using Application;
using Infrastructure.Extentions;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add services to the container.
builder.Services.AddInfrastructure(config);
builder.Services.AddApplication();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

try
{
    app.Logger.LogInformation("Initializing database...");
    await app.Services.ApplyMigrationsAsync();
    app.Logger.LogInformation("Database initialized successfully!");
}
catch (Exception ex)
{
    app.Logger.LogError(ex, "Failed to initialize database");
    throw; // Fallar si no puede crear la DB
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
