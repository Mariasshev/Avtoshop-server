using Data_Access.Context;
using Data_Access.Models;
using Microsoft.EntityFrameworkCore;
using BLL.Interfaces;
using BLL.Services;
using Data_Access.Repositories;

// JWT
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ���������� DbContext � ��������� ������������ ������
builder.Services.AddDbContext<CarContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            sqlOptions.MigrationsAssembly("Data_Access");
            sqlOptions.EnableRetryOnFailure(); 
        }
    )
);


// ������������ ����������� � �������
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

// JWT ��������������
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = "yourIssuer",
        ValidAudience = "yourAudience",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("������������_���������_����_�������32�������"))
    };
});

builder.Services.AddAuthorization();

// ��������� ����������� � swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers().AddJsonOptions(x =>
{
    x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

// CORS - ��������� �� (������ ��� ����������, � ����� ������� ��-�������)
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

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();


// ��������� ���� ���������� �������
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CarContext>();

    // �� �������� EnsureCreated, �.�. ���������� ��������!

    // ��������� ��������, ���� ���
    if (!context.Salers.Any())
    {
        var saler = new Saler
        {
            Name = "����",
            Photo = "https://example.com/saler.jpg",
            Number = "123456789",
            Email = "ivan@example.com",
            Adress = "����, �������"
        };
        context.Salers.Add(saler);
        context.SaveChanges();
    }

    // ��������� ������, ���� ���
    if (!context.Cars.Any())
    {
        var saler = context.Salers.First();

        var cars = new List<Car>
        {
            new Car
            {
                Brand = "Toyota",
                Model = "Corolla",
                Mileage = 98000,
                Year = 2017,
                Transmission = "�������",
                DriverType = "������",
                Condition = "��������",
                EngineSize = 2f,
                FuelType = "Diesel",
                Door = 4,
                Cylinder = 4,
                Color = "׸����",
                VIN = "TESTVIN001",
                Price = 13200,
                Photo = "https://example.com/car1.jpg",
                isOnStock = true,
                SalerId = saler.Id
            },
            new Car
            {
                Brand = "Ford",
                Model = "Focus",
                Mileage = 76000,
                Year = 2019,
                Transmission = "��������",
                DriverType = "��������",
                Condition = "�������",
                EngineSize = 1f,
                FuelType = "Diesel",
                Door = 5,
                Cylinder = 3,
                Color = "�����",
                VIN = "TESTVIN002",
                Price = 14900,
                Photo = "https://example.com/car2.jpg",
                isOnStock = true,
                SalerId = saler.Id
            },
            new Car
            {
                Brand = "BMW",
                Model = "X5",
                Mileage = 45000,
                Year = 2020,
                Transmission = "�������",
                DriverType = "������",
                Condition = "�����",
                EngineSize = 3f,
                FuelType = "Diesel",
                Door = 5,
                Cylinder = 6,
                Color = "�����������",
                VIN = "TESTVIN003",
                Price = 45000,
                Photo = "https://example.com/car3.jpg",
                isOnStock = true,
                SalerId = saler.Id
            },
            new Car
            {
                Brand = "Ford",
                Model = "Focus",
                Mileage = 67000,
                Year = 2018,
                Transmission = "�������",
                DriverType = "��������",
                Condition = "�������",
                EngineSize = 2f,
                FuelType = "Gasoline",
                Door = 4,
                Cylinder = 4,
                Color = "�������",
                VIN = "TESTVIN004",
                Price = 16000,
                Photo = "https://example.com/car4.jpg",
                isOnStock = true,
                SalerId = saler.Id
            },
            new Car
            {
                Brand = "Mercedes-Benz",
                Model = "C-Class",
                Mileage = 53000,
                Year = 2019,
                Transmission = "�������",
                DriverType = "������",
                Condition = "��������",
                EngineSize = 2.2f,
                FuelType = "Diesel",
                Door = 4,
                Cylinder = 4,
                Color = "׸����",
                VIN = "TESTVIN005",
                Price = 38000,
                Photo = "https://example.com/car5.jpg",
                isOnStock = true,
                SalerId = saler.Id
            },
            new Car
            {
                Brand = "Audi",
                Model = "A4",
                Mileage = 42000,
                Year = 2021,
                Transmission = "�������",
                DriverType = "������",
                Condition = "�����",
                EngineSize = 2f,
                FuelType = "Diesel",
                Door = 4,
                Cylinder = 4,
                Color = "�����",
                VIN = "TESTVIN006",
                Price = 40000,
                Photo = "https://example.com/car6.jpg",
                isOnStock = true,
                SalerId = saler.Id
            }
        };

        context.Cars.AddRange(cars);
        context.SaveChanges();
    }
}

app.Run();
