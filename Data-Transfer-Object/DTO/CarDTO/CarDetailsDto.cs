using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Transfer_Object.DTO.Saler;

namespace Data_Transfer_Object.DTO.CarDTO
{
    public class CarDetailsDto
    {
        public int Id { get; set; }
        public int SalerId { get; set; }
        public int Mileage { get; set; }
        public int Year { get; set; }
        public string Transmission { get; set; }
        public string DriverType { get; set; }
        public string Condition { get; set; }
        public int EngineSize { get; set; }
        public int Door { get; set; }
        public int Cylinder { get; set; }
        public string Color { get; set; }
        public string VIN { get; set; }
        public decimal Price { get; set; }
        public string Photo { get; set; }
        public bool isOnStock { get; set; }

        //public SalerDto Saler { get; set; }
    }

}
