using Data_Access.Context;
using Data_Access.Models;
using Data_Transfer_Object.DTO.CarDTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AVTOSHOPWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarsController : ControllerBase
    {
        private readonly CarContext _context;

        public CarsController(CarContext context)
        {
            _context = context;
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
    }
}






//using Data_Access.Context;
//using Data_Access.Models;
//using Data_Transfer_Object.DTO.CarDTO;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace AVTOSHOPWebApi.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class CarsController : ControllerBase
//    {
//        private readonly CarContext _context;

//        public CarsController(CarContext context)
//        {
//            _context = context;
//        }

//        [HttpPost]
//        public async Task<ActionResult> AddCar([FromBody] CarDetailsDto model)

//        {
//            var car = new Car
//            {
//                Mileage = model.Mileage,
//                Year = model.Year,
//                Transmission = model.Transmission,
//                DriverType = model.DriverType,
//                Condition = model.Condition,
//                EngineSize = model.EngineSize,
//                Door = model.Door,
//                Cylinder = model.Cylinder,
//                Color = model.Color,
//                VIN = model.VIN,
//                Price = model.Price,
//                Photo = model.Photo,
//                isOnStock = model.isOnStock,
//                SalerId = model.SalerId
//                //Saler.SalerId = model.SalerId
//            };

//            _context.Cars.Add(car);
//            await _context.SaveChangesAsync();

//            return Ok(car);
//        }


//        [HttpGet("list")]
//        public async Task<ActionResult<IEnumerable<CarListItemDto>>> GetCarList()
//        {
//            var cars = await _context.Cars
//                .Select(car => new CarListItemDto
//                {
//                    Id = car.Id,
//                    Photo = car.Photo,
//                    Title = car.Brand + " " + car.Model + " " + car.Year, // Пример: Toyota Corolla 2020
//                    Badge = car.Condition, // Или другой текст, например "New"
//                    Price = car.Price,
//                    Transmission = car.Transmission,
//                    Year = car.Year,
//                    Mileage = car.Mileage
//                })
//                .ToListAsync();

//            return Ok(cars);
//        }


//    }
//}
