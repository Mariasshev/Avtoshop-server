using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

public class CarCreateDto
{
    public int Mileage { get; set; }
    public int Year { get; set; }
    public string Transmission { get; set; }
    public string FuelType { get; set; }
    public string Brand { get; set; }
    public string Model { get; set; }
    public string DriverType { get; set; }
    public string Condition { get; set; }
    public float EngineSize { get; set; }
    public int Door { get; set; }
    public int Cylinder { get; set; }
    public string Color { get; set; }
    public string VIN { get; set; }
    public float Price { get; set; }

    // Для загрузки нескольких файлов
    public List<IFormFile> Photos { get; set; }
    public string? Description { get; set; }

}
