using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

Env.Load();

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

// Configure services for endpoints and Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// Agregar el consumidor al contenedor de servicios
builder.Services.AddSingleton<BranchProductQuantityUpdateConsumer>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("AllowAll");
}

app.UseHttpsRedirection();

// Map endpoints to controllers
app.MapControllers();

// Iniciar el consumidor
using (var scope = app.Services.CreateScope())
{
    var consumer = scope.ServiceProvider.GetRequiredService<BranchProductQuantityUpdateConsumer>();
    consumer.StartListening();
}

app.Run();
