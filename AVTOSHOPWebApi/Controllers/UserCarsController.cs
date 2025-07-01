//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Authorization;
//using System.IdentityModel.Tokens.Jwt;
//using Data_Access.Repositories;
//using System.Security.Claims;
//using Data_Transfer_Object.DTO.UserDTO;
//using Data_Transfer_Object.DTO.CarDTO;
//using Data_Access.Models;
//using Microsoft.AspNetCore.Identity;

//namespace AVTOSHOPWebApi.Controllers
//{

//    [Authorize] // Только авторизованные могут добавлять
//    [HttpPost("add-with-photo")]
//    public async Task<IActionResult> AddCarWithPhoto([FromForm] CarCreateDto model)
//    {
//        if (model == null) return BadRequest("Invalid car data");

//        string filePath = null;

//        if (model.Photo != null && model.Photo.Length > 0)
//        {
//            var uploadsFolder = Path.Combine("wwwroot", "uploads", "cars");
//            if (!Directory.Exists(uploadsFolder))
//                Directory.CreateDirectory(uploadsFolder);

//            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(model.Photo.FileName)}";
//            filePath = Path.Combine(uploadsFolder, fileName);

//            using (var stream = new FileStream(filePath, FileMode.Create))
//            {
//                await model.Photo.CopyToAsync(stream);
//            }
//        }

//        var car = new Car
//        {
//            Mileage = model.Mileage,
//            Year = model.Year,
//            Transmission = model.Transmission,
//            Brand = model.Brand,
//            Model = model.Model,
//            FuelType = model.FuelType,
//            DriverType = model.DriverType,
//            Condition = model.Condition,
//            EngineSize = model.EngineSize,
//            Door = model.Door,
//            Cylinder = model.Cylinder,
//            Color = model.Color,
//            VIN = model.VIN,
//            Price = model.Price,
//            isOnStock = model.isOnStock,
//            // SalerId пока хардкод или из токена
//            SalerId = 1, // временно
//            Photo = filePath != null ? "/uploads/cars/" + Path.GetFileName(filePath) : null
//        };

//        _context.Cars.Add(car);
//        await _context.SaveChangesAsync();

//        return Ok(new { message = "Car added successfully!", id = car.Id });
//    }
//}
