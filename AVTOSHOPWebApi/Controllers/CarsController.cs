using Data_Access.Context;
using Data_Access.Models;
using Data_Transfer_Object.DTO.CarDTO;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BLL.Services;

namespace AVTOSHOPWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarsController : ControllerBase
    {
        private readonly CarContext _context;
        private readonly ICarService _carService;

        public CarsController(CarContext context, ICarService carService)
        {
            _context = context;
            _carService = carService;
        }

        // POST: api/cars
        // Добавление новой машины
        [HttpPost]
        public async Task<ActionResult<Car>> AddCar([FromBody] CarDetailsDto model)
        {
            if (model == null)
                return BadRequest("Invalid car data");

            var car = new Car
            {
                Mileage = model.Mileage,
                Year = model.Year,
                Transmission = model.Transmission,
                Brand = model.Brand,
                Model = model.Model,
                FuelType = model.FuelType,
                DriverType = model.DriverType,
                Condition = model.Condition,
                EngineSize = model.EngineSize,
                Door = model.Door,
                Cylinder = model.Cylinder,
                Color = model.Color,
                VIN = model.VIN,
                Price = model.Price,
                Photo = model.Photo,
                isOnStock = model.isOnStock,
                SalerId = model.SalerId
            };

            _context.Cars.Add(car);
            await _context.SaveChangesAsync();

            // Возвращаем созданный объект с кодом 201 Created и заголовком Location
            return CreatedAtAction(nameof(GetCarById), new { id = car.Id }, car);
        }

        // GET: api/cars
        // Получение списка всех машин
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CarListItemDto>>> GetCarList()
        {
            var cars = await _context.Cars
                .Select(car => new CarListItemDto
                {
                    Id = car.Id,
                    Photo = car.Photo,
                    Brand = car.Brand,
                    Model = car.Model,
                    Title = car.Brand + " " + car.Model + " " + car.Year,
                    Badge = car.Condition,
                    Fuel = car.FuelType,
                    Price = car.Price,
                    Transmission = car.Transmission,
                    Year = car.Year,
                    Mileage = car.Mileage
                })
                .ToListAsync();

            return Ok(cars);
        }

        // GET: api/cars/{id}
        // Получение машины по ID (полезно для CreatedAtAction)
        [HttpGet("{id}")]
        public async Task<ActionResult<CarListItemDto>> GetCarById(int id)
        {
            var car = await _context.Cars
                .Where(c => c.Id == id)
                .Select(car => new CarListItemDto
                {
                    Id = car.Id,
                    Photo = car.Photo,
                    Brand = car.Brand,
                    Model = car.Model,
                    Title = car.Brand + " " + car.Model + " " + car.Year,
                    Badge = car.Condition,
                    Fuel = car.FuelType,
                    Price = car.Price,
                    Transmission = car.Transmission,
                    Year = car.Year,
                    Mileage = car.Mileage
                })
                .FirstOrDefaultAsync();

            if (car == null)
                return NotFound();

            return Ok(car);
        }


        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> AddCarWithPhotos([FromForm] CarCreateDto dto)
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userId");
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                    return Unauthorized("User not found");

                var response = await _carService.AddCarWithPhotosAsync(dto, userId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении машины: {ex}");
                return StatusCode(500, "Server error");
            }
        }



        [HttpGet("user-cars")]
        [Authorize]
        public async Task<IActionResult> GetUserCars()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return Unauthorized("User not found");
            

            userId = int.Parse(userIdClaim.Value);

            var saler = await _context.Salers.FirstOrDefaultAsync(s => s.UserId == userId);
            if (saler == null)
                return NotFound("Продавец не найден");

            var cars = await _context.Cars
                .Where(c => c.SalerId == saler.Id)
                .ToListAsync();

            return Ok(cars);
        }



    }


}

