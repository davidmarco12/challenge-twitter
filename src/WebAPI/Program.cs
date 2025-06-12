using Infrastructure;
using Application;
using Infrastructure.Extentions;
using WebAPI.Middlewares;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
var aspnetUrls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS");

if (!string.IsNullOrEmpty(aspnetUrls))
{
    // Para docker-compose
    Console.WriteLine($"Docker mode - Using: {aspnetUrls}");
}
else
{
    // Railway o local - configurar manualmente
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddInfrastructure(config);
builder.Services.AddApplication();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(policy =>
{
    policy.AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader();
});

if (!builder.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => "OK");

try
{
    await app.Services.ApplyMigrationsAsync();
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Failed to initialize database on Railway");
    // En Railway, no deberia fallar el startup si la DB no está lista inmediatamente
}

app.Run();
