using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Http.StatusCodes;

var builder = WebApplication.CreateBuilder(args);


var Configuration = builder.Configuration;
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

// Configure services for endpoints and Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Add controllers
builder.Services.AddControllers();


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

app.Run();


