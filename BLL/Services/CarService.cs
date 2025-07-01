using BLL.Interfaces;
using Data_Access.Context;
using Data_Access.Models;
using Data_Transfer_Object.DTO.CarDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services
{
    public class CarService : ICarService
    {
        private readonly CarContext _context;

        public CarService(CarContext context)
        {
            _context = context;
        }

        public async Task<CarResponseDto> AddCarWithPhotosAsync(CarCreateDto dto, int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId)
                       ?? throw new Exception("User not found");

            var saler = await _context.Salers.FirstOrDefaultAsync(s => s.UserId == userId);
            if (saler == null)
            {
                saler = new Saler
                {
                    UserId = userId,
                    Name = user.Name ?? "Без имени",
                    Email = user.Email,
                    Number = user.PhoneNumber ?? "",
                    Adress = $"{user.City ?? ""}, {user.Country ?? ""}",
                    Photo = user.PhotoUrl ?? ""
                };
                _context.Salers.Add(saler);
            }

            var car = new Car
            {
                Mileage = dto.Mileage,
                Year = dto.Year,
                Transmission = dto.Transmission,
                FuelType = dto.FuelType,
                Brand = dto.Brand,
                Model = dto.Model,
                DriverType = dto.DriverType,
                Condition = dto.Condition,
                EngineSize = dto.EngineSize,
                Door = dto.Door,
                Cylinder = dto.Cylinder,
                Color = dto.Color,
                VIN = dto.VIN,
                Price = dto.Price,
                Saler = saler,
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Photo = ""
            };
            _context.Cars.Add(car);

            if (dto.Photos != null && dto.Photos.Any())
            {
                var uploadsFolder = Path.Combine("wwwroot", "uploads", "car_photos", userId.ToString());
                Directory.CreateDirectory(uploadsFolder);

                var photoUrls = new List<string>();

                foreach (var photo in dto.Photos)
                {
                    var ext = Path.GetExtension(photo.FileName);
                    var fileName = $"{Guid.NewGuid()}{ext}";
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await photo.CopyToAsync(stream);

                    var relativePath = $"/uploads/car_photos/{userId}/{fileName}";
                    photoUrls.Add(relativePath);

                    _context.CarPhotos.Add(new CarPhoto
                    {
                        Car = car,
                        PhotoUrl = relativePath
                    });
                }

                car.Photo = photoUrls.First();
                car.UpdatedAt = DateTime.UtcNow;
            }


            var carPhotos = await _context.CarPhotos
                .Where(cp => cp.Car.Id == car.Id)
                .Select(cp => cp.PhotoUrl)
                .ToListAsync();

            await _context.SaveChangesAsync();

            return new CarResponseDto
            {
                Id = car.Id,
                Brand = car.Brand,
                Model = car.Model,
                Price = car.Price,
                Photo = car.Photo,
                Photos = carPhotos,
                CreatedAt = car.CreatedAt,
                UpdatedAt = car.UpdatedAt
            };
        }


        public async Task<CarResponseDto> UpdateCarWithPhotosAsync(int carId, CarCreateDto dto, int userId)
        {
            var car = await _context.Cars
                .Include(c => c.CarPhotos) // если у тебя есть коллекция CarPhotos
                .FirstOrDefaultAsync(c => c.Id == carId && c.Saler.UserId == userId);

            if (car == null)
                throw new Exception("Car not found or access denied");

            // обновляем поля
            car.Mileage = dto.Mileage;
            car.Year = dto.Year;
            car.Transmission = dto.Transmission;
            car.FuelType = dto.FuelType;
            car.Brand = dto.Brand;
            car.Model = dto.Model;
            car.DriverType = dto.DriverType;
            car.Condition = dto.Condition;
            car.EngineSize = dto.EngineSize;
            car.Door = dto.Door;
            car.Cylinder = dto.Cylinder;
            car.Color = dto.Color;
            car.VIN = dto.VIN;
            car.Price = dto.Price;
            car.Description = dto.Description;
            car.UpdatedAt = DateTime.UtcNow;

            // если загружены новые фото
            if (dto.Photos != null && dto.Photos.Any())
            {
                var uploadsFolder = Path.Combine("wwwroot", "uploads", "car_photos", userId.ToString());
                Directory.CreateDirectory(uploadsFolder);

                var photoUrls = new List<string>();

                foreach (var photo in dto.Photos)
                {
                    var ext = Path.GetExtension(photo.FileName);
                    var fileName = $"{Guid.NewGuid()}{ext}";
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await photo.CopyToAsync(stream);

                    var relativePath = $"/uploads/car_photos/{userId}/{fileName}";
                    photoUrls.Add(relativePath);

                    _context.CarPhotos.Add(new CarPhoto
                    {
                        CarId = car.Id,
                        PhotoUrl = relativePath
                    });
                }

                // основное фото для карточки
                car.Photo = photoUrls.First();
            }

            await _context.SaveChangesAsync();

            return new CarResponseDto
            {
                Id = car.Id,
                Brand = car.Brand,
                Model = car.Model,
                Price = car.Price,
                Photo = car.Photo,
                //Description = car.Description,
                CreatedAt = car.CreatedAt,
                UpdatedAt = car.UpdatedAt
            };
        }

        public async Task<CarResponseDto> GetCarByIdAsync(int id)
        {
            var car = await _context.Cars
                .Include(c => c.CarPhotos)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (car == null)
                return null;

            return new CarResponseDto
            {
                Id = car.Id,
                Brand = car.Brand,
                Model = car.Model,
                Price = car.Price,
                Photo = car.Photo,
                Photos = car.CarPhotos?.Select(cp => cp.PhotoUrl).ToList() ?? new List<string>(),
                CreatedAt = car.CreatedAt,
                UpdatedAt = car.UpdatedAt,
                // Добавь остальные нужные поля
            };
        }



        public async Task<CarResponseDto> UpdateCarAsync(CarUpdateDto dto)
        {
            var car = await _context.Cars.FirstOrDefaultAsync(c => c.Id == dto.Id);
            if (car == null) return null;

            // обнови нужные поля, например
            car.Brand = dto.Brand;
            car.Model = dto.Model;
            // и т.д.

            await _context.SaveChangesAsync();

            return new CarResponseDto
            {
                Id = car.Id,
                Brand = car.Brand,
                Model = car.Model,
                Price = car.Price,
                Photo = car.Photo,
                CreatedAt = car.CreatedAt,
                UpdatedAt = car.UpdatedAt
            };
        }



    }
}
