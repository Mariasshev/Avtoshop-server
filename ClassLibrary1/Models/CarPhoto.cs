using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access.Models
{
    public class CarPhoto
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public string PhotoUrl { get; set; }

        // Навигационное свойство
        public Car Car { get; set; }
    }
}

