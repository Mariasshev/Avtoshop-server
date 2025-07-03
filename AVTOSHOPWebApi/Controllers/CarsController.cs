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


        // GET: api/cars/filter-cars
        [HttpGet("filter-cars")]
        public async Task<IActionResult> GetFilteredCars(
            [FromQuery] int? brandId,
            [FromQuery] string? modelName,
            [FromQuery] string? transmission,
            [FromQuery] string? fuelType,
            [FromQuery] string? color,
            [FromQuery] string? driverType,
            [FromQuery] string? condition,
            [FromQuery] int? yearMin,
            [FromQuery] int? yearMax,
            [FromQuery] float? engineSizeMin,
            [FromQuery] float? engineSizeMax,
            [FromQuery] int? door,
            [FromQuery] int? cylinder,
            [FromQuery] int? mileage,
            [FromQuery] float? priceMin,
            [FromQuery] float? priceMax,
            [FromQuery] bool? hasVin
        )
        {
            var query = _context.Cars.Include(c => c.Brand).AsQueryable();

            if (brandId.HasValue)
                query = query.Where(c => c.BrandId == brandId.Value);

            if (!string.IsNullOrEmpty(modelName))
                query = query.Where(c => c.Model == modelName);

            if (!string.IsNullOrEmpty(transmission))
                query = query.Where(c => c.Transmission == transmission);

            if (!string.IsNullOrEmpty(fuelType))
                query = query.Where(c => c.FuelType == fuelType);

            if (!string.IsNullOrEmpty(color))
                query = query.Where(c => c.Color == color);

            if (!string.IsNullOrEmpty(driverType))
                query = query.Where(c => c.DriverType == driverType);

            if (!string.IsNullOrEmpty(condition))
                query = query.Where(c => c.Condition == condition);

            if (yearMin.HasValue)
                query = query.Where(c => c.Year >= yearMin.Value);

            if (yearMax.HasValue)
                query = query.Where(c => c.Year <= yearMax.Value);

            if (engineSizeMin.HasValue)
                query = query.Where(c => c.EngineSize >= engineSizeMin.Value);

            if (engineSizeMax.HasValue)
                query = query.Where(c => c.EngineSize <= engineSizeMax.Value);

            if (door.HasValue)
                query = query.Where(c => c.Door == door.Value);

            if (cylinder.HasValue)
                query = query.Where(c => c.Cylinder == cylinder.Value);

            if (mileage.HasValue)
                query = query.Where(c => c.Mileage <= mileage.Value);

            if (priceMin.HasValue)
                query = query.Where(c => c.Price >= priceMin.Value);

            if (priceMax.HasValue)
                query = query.Where(c => c.Price <= priceMax.Value);

            if (hasVin.HasValue && hasVin.Value)
                query = query.Where(c => !string.IsNullOrEmpty(c.VIN));

            var result = await query.ToListAsync();

            return Ok(result);
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
                BrandId = model.BrandId,
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
                    Brand = car.Brand.Name,
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
        public async Task<ActionResult<CarDetailsDto>> GetCarById(int id)
        {
            var car = await _context.Cars
                .Where(c => c.Id == id)
                .Select(car => new CarDetailsDto
                {
                    Id = car.Id,
                    Brand = car.Brand.Name,
                    Model = car.Model,
                    FuelType = car.FuelType,
                    Price = car.Price,
                    Transmission = car.Transmission,
                    Year = car.Year,
                    Mileage = car.Mileage,
                    BrandId = car.BrandId,
                    DriverType = car.DriverType,
                    Condition = car.Condition,
                    EngineSize = car.EngineSize,
                    Door = car.Door,
                    Cylinder = car.Cylinder,
                    Color = car.Color,
                    VIN = car.VIN,
                    Photo = car.Photo,
                    isOnStock = car.isOnStock,
                    SalerId = car.SalerId,
                    Description = car.Description,

                    SalerName = car.Saler.Name,
                    SalerPhoto = car.Saler.Photo,
                    SalerAddress = car.Saler.Adress,
                    SalerPhone = car.Saler.Number

                })
                .FirstOrDefaultAsync();

            if (car == null)
                return NotFound();

            return Ok(car);
        }


        [HttpGet("{id}/details")]
        public async Task<ActionResult<CarCreateDto>> GetCarDetails(int id)
        {
            var car = await _context.Cars
                .Include(c => c.CarPhotos)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (car == null) return NotFound();

            var dto = new CarCreateDto
            {
                Id = car.Id,
                Mileage = car.Mileage,
                Year = car.Year,
                Transmission = car.Transmission,
                FuelType = car.FuelType,
                BrandId = car.BrandId,
                //Brand = car.Brand.Name,
                Model = car.Model,
                DriverType = car.DriverType,
                Condition = car.Condition,
                EngineSize = car.EngineSize,
                Door = car.Door,
                Cylinder = car.Cylinder,
                Color = car.Color,
                VIN = car.VIN,
                Price = car.Price,
                Description = car.Description,
                PhotoUrls = car.CarPhotos?.Select(cp => cp.PhotoUrl).ToList() ?? new List<string>()
            };

            return Ok(dto);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCar(int id, [FromForm] CarCreateDto dto)
        {
            if (id != dto.Id)
                return BadRequest(new { error = "Id in URL and body do not match" });

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userId");
            if (userIdClaim == null)
                return Unauthorized();

            if (!int.TryParse(userIdClaim.Value, out int userId))
                return Unauthorized();

            List<string> photosToDeleteList = new();
            if (!string.IsNullOrEmpty(dto.PhotosToDelete))
            {
                photosToDeleteList = System.Text.Json.JsonSerializer.Deserialize<List<string>>(dto.PhotosToDelete);
                Console.WriteLine("PhotosToDelete JSON: " + dto.PhotosToDelete);
                Console.WriteLine("Deserialized list:");
                foreach (var p in photosToDeleteList)
                {
                    Console.WriteLine("- " + p);
                }
            }


            try
            {
                var updatedCar = await _carService.UpdateCarWithPhotosAsync(id, dto, userId, photosToDeleteList);

                if (updatedCar == null)
                    return NotFound();

                return Ok(updatedCar);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
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



        // DELETE api/cars/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCar(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null)
                return NotFound(); // Объявление не найдено

            _context.Cars.Remove(car);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Логируем ошибку, если нужно
                return StatusCode(500, "Ошибка при удалении объявления");
            }

            return NoContent(); // Успешно удалено, возвращаем 204 No Content
        }



    }


}

