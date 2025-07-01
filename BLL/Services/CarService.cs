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
            Console.WriteLine("AddCarWithPhotosAsync START");
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
                await _context.SaveChangesAsync(); // Сохраняем, чтобы saler.Id был доступен
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
                SalerId = saler.Id,
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Photo = ""
            };
            _context.Cars.Add(car);
            await _context.SaveChangesAsync(); // Сохраняем, чтобы получить car.Id

            if (dto.Photos != null && dto.Photos.Any())
            {
                var uploadsFolder = Path.Combine("wwwroot", "uploads", "car_photos", userId.ToString(), car.Id.ToString());
                Directory.CreateDirectory(uploadsFolder);

                var photoUrls = new List<string>();

                foreach (var photo in dto.Photos)
                {
                    var ext = Path.GetExtension(photo.FileName);
                    var fileName = $"{Guid.NewGuid()}{ext}";
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await photo.CopyToAsync(stream);

                    var relativePath = $"/uploads/car_photos/{userId}/{car.Id}/{fileName}";
                    photoUrls.Add(relativePath);

                    _context.CarPhotos.Add(new CarPhoto
                    {
                        CarId = car.Id,
                        PhotoUrl = relativePath
                    });
                }

                car.Photo = photoUrls.First();
                car.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
            }

            var carPhotos = await _context.CarPhotos
                .Where(cp => cp.CarId == car.Id)
                .Select(cp => cp.PhotoUrl)
                .ToListAsync();

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


        public async Task<CarResponseDto> UpdateCarWithPhotosAsync(int carId, CarCreateDto dto, int userId, List<string> photosToDelete)
        {
            Console.WriteLine("UpdateCarWithPhotosAsync START");

            var car = await _context.Cars
                .Include(c => c.CarPhotos)
                .Include(c => c.Saler)
                .FirstOrDefaultAsync(c => c.Id == carId && c.Saler.UserId == userId);

            if (car == null)
                throw new Exception("Car not found or access denied");

            // Обновляем поля машины
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

            // Удаляем фотографии, если есть список для удаления

            Console.WriteLine($"photosToDelete is null: {photosToDelete }");
            //Console.WriteLine($"photosToDelete count: {photosToDelete?.Count ?? 0}");
            if (photosToDelete != null && photosToDelete.Count > 0)
            {

                var photosForDelete = car.CarPhotos.Where(cp => photosToDelete.Contains(cp.PhotoUrl)).ToList();

                foreach (var photo in photosForDelete)
                {
                    var physicalPath = Path.Combine("wwwroot", photo.PhotoUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                    Console.WriteLine($"Deleting file: {physicalPath}");
                    if (File.Exists(physicalPath))
                    {
                        try
                        {
                            File.Delete(physicalPath);
                            Console.WriteLine($"Deleted file: {physicalPath}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error deleting file {physicalPath}: {ex.Message}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"File does not exist: {physicalPath}");
                    }

                    _context.CarPhotos.Remove(photo);
                    Console.WriteLine($"Removed photo record: {photo.PhotoUrl}");
                }
            }


            // Добавляем новые фото
            if (dto.Photos != null && dto.Photos.Any())
            {
                var uploadsFolder = Path.Combine("wwwroot", "uploads", "car_photos", userId.ToString(), car.Id.ToString());
                Directory.CreateDirectory(uploadsFolder);

                var photoUrls = new List<string>();

                foreach (var photo in dto.Photos)
                {
                    var ext = Path.GetExtension(photo.FileName);
                    var fileName = $"{Guid.NewGuid()}{ext}";
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await photo.CopyToAsync(stream);

                    var relativePath = $"/uploads/car_photos/{userId}/{car.Id}/{fileName}";
                    photoUrls.Add(relativePath);

                    _context.CarPhotos.Add(new CarPhoto
                    {
                        CarId = car.Id,
                        PhotoUrl = relativePath
                    });
                }

                // Обновляем основное фото (главную)
                car.Photo = photoUrls.First();
            }
            else if (car.CarPhotos.Any())
            {
                // Если новых фото нет, но есть старые - устанавливаем первое в качестве основного
                car.Photo = car.CarPhotos.First().PhotoUrl;
            }
            else
            {
                // Если фоток вообще нет, очищаем
                car.Photo = "";
            }

            await _context.SaveChangesAsync();

            // Формируем DTO для ответа
            var photosList = await _context.CarPhotos
                .Where(cp => cp.CarId == car.Id)
                .Select(cp => cp.PhotoUrl)
                .ToListAsync();

            return new CarResponseDto
            {
                Id = car.Id,
                Brand = car.Brand,
                Model = car.Model,
                Price = car.Price,
                Photo = car.Photo,
                Photos = photosList,
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
