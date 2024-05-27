using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Http.StatusCodes;

var builder = WebApplication.CreateBuilder(args);

var Configuration = builder.Configuration;

// Obtener las variables de entorno
var postgresHost = Environment.GetEnvironmentVariable("POSTGRES_HOST");
var port = Environment.GetEnvironmentVariable("POSTGRES_PORT");
var postgresDb = Environment.GetEnvironmentVariable("POSTGRES_DB");
var postgresUser = Environment.GetEnvironmentVariable("POSTGRES_USER");
var postgresPassword = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");

// Configurar la cadena de conexión utilizando las variables de entorno
var connectionString = $"Server={postgresHost};Port={port};Database={postgresDb};Username={postgresUser};Password={postgresPassword}";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Configurar el registro
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
});

// Configurar RabbitMQ
builder.Services.AddHostedService<RabbitMqConsumerService>();

// Configurar servicios para endpoints y Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Add controllers
builder.Services.AddControllers();


var app = builder.Build();

// Configurar el pipeline de solicitud HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("AllowAll");
}

app.UseHttpsRedirection();

// Mapear endpoints a controladores
app.MapControllers();

// Iniciar el consumidor
using (var scope = app.Services.CreateScope())
{
    var consumer = scope.ServiceProvider.GetRequiredService<BranchProductQuantityUpdateConsumer>();
    consumer.StartListening();
}

app.Run();
