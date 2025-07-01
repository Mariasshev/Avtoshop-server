using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Transfer_Object.DTO.CarDTO
{
    public class CarResponseDto
    {
        public int Id { get; set; }
        public string Brand { get; set; } = "";
        public string Model { get; set; } = "";
        public float Price { get; set; }
        public string Photo { get; set; } // Первое фото
        public List<string> Photos { get; set; } = new List<string>(); // Все фото
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
