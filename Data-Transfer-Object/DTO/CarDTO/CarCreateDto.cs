using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

public class CarCreateDto
{
    public int Id { get; set; }
    public int Mileage { get; set; }
    public int Year { get; set; }
    public string Transmission { get; set; }
    public string FuelType { get; set; }

    public int BrandId { get; set; }

    //public string Brand { get; set; }
    public string Model { get; set; }
    public string DriverType { get; set; }
    public string Condition { get; set; }
    public float EngineSize { get; set; }
    public int Door { get; set; }
    public int Cylinder { get; set; }
    public string Color { get; set; }
    public string VIN { get; set; }
    public float Price { get; set; }

    // Для загрузки нескольких 
    public List<IFormFile>? Photos { get; set; } // новые файлы
    public List<string>? PhotoUrls { get; set; }  // для передачи путей к фотографиям клиенту

    public string? PhotosToDelete { get; set; }
    public string? Description { get; set; }

}
