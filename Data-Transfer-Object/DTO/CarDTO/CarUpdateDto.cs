﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Transfer_Object.DTO.CarDTO
{
    public class CarUpdateDto
    {
        public int Id { get; set; }  // обязательно для обновления
        public int Mileage { get; set; }
        public int Year { get; set; }
        public string Transmission { get; set; }
        public string FuelType { get; set; }
        public int BrandId { get; set; }
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
        public string? Description { get; set; }

        // Если обновление с файлами тоже нужно
        public List<IFormFile> Photos { get; set; }
    }

}
