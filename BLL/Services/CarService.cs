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

            // Проверяем пользователя
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId)
                       ?? throw new Exception("User not found");

            // Проверяем, существует ли бренд с таким BrandId
            var brandExists = await _context.Brands.AnyAsync(b => b.Id == dto.BrandId);
            if (!brandExists)
                throw new Exception($"Brand with Id {dto.BrandId} not found");



            // Проверяем наличие Saler и создаём, если нужно
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
                await _context.SaveChangesAsync(); // Чтобы получить saler.Id
            }

            // Создаём машину с указанным BrandId
            var car = new Car
            {
                Mileage = dto.Mileage,
                Year = dto.Year,
                Transmission = dto.Transmission,
                FuelType = dto.FuelType,
                BrandId = dto.BrandId,
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
            await _context.SaveChangesAsync(); // Получаем car.Id

            // Если есть фото — сохраняем
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

            // Загружаем машину с брендом (включаем Brand)
            var savedCar = await _context.Cars
                .Include(c => c.Brand)
                .FirstOrDefaultAsync(c => c.Id == car.Id);

            // Загружаем фото для DTO
            var carPhotos = await _context.CarPhotos
                .Where(cp => cp.CarId == car.Id)
                .Select(cp => cp.PhotoUrl)
                .ToListAsync();

            return new CarResponseDto
            {
                Id = savedCar.Id,
                Brand = savedCar.Brand?.Name ?? "Неизвестно",   // Название бренда из навигационного свойства
                Model = savedCar.Model,
                Price = savedCar.Price,
                Photo = savedCar.Photo,
                Photos = carPhotos,
                CreatedAt = savedCar.CreatedAt,
                UpdatedAt = savedCar.UpdatedAt
            };
        }




        public async Task<CarResponseDto> UpdateCarWithPhotosAsync(int carId, CarCreateDto dto, int userId, List<string> photosToDelete)
        {
            Console.WriteLine("UpdateCarWithPhotosAsync START");

            var car = await _context.Cars
             .Include(c => c.Brand)
             .Include(c => c.CarPhotos)
             .FirstOrDefaultAsync(c => c.Id == carId);

            Console.WriteLine($"UpdateCarWithPhotosAsync: Loaded car.Brand: {(car.Brand != null ? car.Brand.Name : "null")}");

            if (car == null)
                throw new Exception("Car not found or access denied");

            // Обновляем поля машины
            car.Mileage = dto.Mileage;
            car.Year = dto.Year;
            car.Transmission = dto.Transmission;
            car.FuelType = dto.FuelType;
            car.BrandId = dto.BrandId;
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
                Brand = car.Brand.Name,
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

            Console.WriteLine($"GetCarByIdAsync: car.Brand: {(car.Brand != null ? car.Brand.Name : "null")}");


            if (car == null)
                return null;

            return new CarResponseDto
            {
                Id = car.Id,
                Brand = car.Brand.Name,
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
            var car = await _context.Cars
                .Include(c => c.Brand) // обязательно включаем навигационное свойство
                .FirstOrDefaultAsync(c => c.Id == dto.Id);
            Console.WriteLine($"UpdateCarAsync: car.Brand: {(car.Brand != null ? car.Brand.Name : "null")}");
            if (car == null) return null;

            // обновляем нужные поля
            car.BrandId = dto.BrandId; // теперь BrandId
            car.Model = dto.Model;
            // обнови и другие поля, если нужно, например:
            car.Price = dto.Price;
            car.Description = dto.Description;
            car.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new CarResponseDto
            {
                Id = car.Id,
                Brand = car.Brand.Name,   // теперь возвращаем название бренда
                Model = car.Model,
                Price = car.Price,
                Photo = car.Photo,
                CreatedAt = car.CreatedAt,
                UpdatedAt = car.UpdatedAt
            };
        }

    }
}
