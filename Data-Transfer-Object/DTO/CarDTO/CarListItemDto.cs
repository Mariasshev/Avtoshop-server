using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Transfer_Object.DTO.CarDTO
{
    public class CarListItemDto
    {
        public int Id { get; set; }
        public string Photo { get; set; }
        public string Title { get; set; }  // "Toyota Corolla 2020"
        public string Brand { get; set; }  // "Toyota Corolla 2020"
        public string Model { get; set; }  // "Toyota Corolla 2020"

        public string Fuel { get; set; }
        public string Badge { get; set; }
        public float Price { get; set; }
        public string Transmission { get; set; }
        public int Year { get; set; }
        public int Mileage { get; set; }
    }
}
