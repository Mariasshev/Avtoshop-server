using Data_Access.Context;
using Data_Access.Models;
using Data_Transfer_Object.DTO.CarDTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        [HttpPost]
        public async Task<ActionResult> AddCar([FromBody] CarDetailsDto model)

        {
            var car = new Car
            {
                Mileage = model.Mileage,
                Year = model.Year,
                Transmission = model.Transmission,
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
                //Saler.SalerId = model.SalerId
            };

            _context.Cars.Add(car);
            await _context.SaveChangesAsync();

            return Ok(car);
        }

    }
}
