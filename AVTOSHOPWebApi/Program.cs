using Data_Access.Context;
using Data_Access.Models;
using Microsoft.EntityFrameworkCore;
using BLL.Interfaces;
using BLL.Services;
using Data_Access.Repositories;


var builder = WebApplication.CreateBuilder(args);
// Добавляем DbContext
builder.Services.AddDbContext<CarContext>(options =>
    options.UseSqlServer("Server=LAPTOP-RPD6D51R\\SQLEXPRESS01;Database=CarDatabase;Trusted_Connection=True;TrustServerCertificate=true"));

// Регистрация репозитория
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Регистрация сервиса
builder.Services.AddScoped<IUserService, UserService>();


// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers().AddJsonOptions(x =>
{
    x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();
app.UseCors("AllowAll");


using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CarContext>();
    context.Database.EnsureCreated();
}

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CarContext>();
    context.Database.EnsureCreated();

    if (!context.Salers.Any())
    {
        var saler = new Saler
        {
            Name = "Иван",
            Photo = "https://example.com/saler.jpg", // если нужно
            Number = "123456789",
            Email = "ivan@example.com",
            Adress = "Киев, Украина"
        };

        context.Salers.Add(saler);
        context.SaveChanges();
    }

    if (!context.Cars.Any())
    {
        var saler = context.Salers.First();

        var car1 = new Car
        {
            Mileage = 98000,
            Year = 2017,
            Transmission = "Автомат",
            DriverType = "Полный",
            Condition = "Отличное",
            EngineSize = 2,
            Door = 4,
            Cylinder = 4,
            Color = "Чёрный",
            VIN = "TESTVIN001",
            Price = 13200,
            Photo = "https://example.com/car1.jpg",
            isOnStock = true,
            SalerId = saler.Id
        };

        var car2 = new Car
        {
            Mileage = 76000,
            Year = 2019,
            Transmission = "Механика",
            DriverType = "Передний",
            Condition = "Хорошее",
            EngineSize = 1,
            Door = 5,
            Cylinder = 3,
            Color = "Белый",
            VIN = "TESTVIN002",
            Price = 14900,
            Photo = "https://example.com/car2.jpg",
            isOnStock = true,
            SalerId = saler.Id
        };

        context.Cars.AddRange(car1, car2);
        context.SaveChanges();
    }
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
