using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access.Models
{
    public class Car
    {
        public int Id { get; set; }
        public int Mileage { get; set; }
        public int Year { get; set; }
        public string Transmission { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string DriverType { get; set; }
        public string FuelType { get; set; }
        public string Condition { get; set; }
        public float EngineSize { get; set; }
        public int Door { get; set; }
        public int Cylinder { get; set; }
        public string Color { get; set; }
        public string VIN { get; set; }
        public float Price { get; set; }
        public string Photo { get; set; }
        public bool isOnStock { get; set; }
        public int SalerId { get; set; }

        //public Saler Saler { get; set; }
        //public int? OrderId { get; set; }

        [ForeignKey("SalerId")]
        public Saler? Saler { get; set; } // сделай nullable и без [Required]

        public Order Order { get; set; }

    }
}
