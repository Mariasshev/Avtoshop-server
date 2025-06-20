using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access.Models
{
    public class CarBrands
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public ICollection<CarModel> Models { get; set; } = new List<CarModel>();
    }
}
