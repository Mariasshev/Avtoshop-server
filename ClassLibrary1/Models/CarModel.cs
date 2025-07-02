using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access.Models
{
    public class CarModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int BrandId { get; set; }
        public CarBrands Brand { get; set; }
    }
}
