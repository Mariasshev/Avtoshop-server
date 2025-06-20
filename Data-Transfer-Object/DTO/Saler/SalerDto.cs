using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Transfer_Object.DTO.Saler
{
    public class SalerDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Photo { get; set; }
        public string Number { get; set; }
        public string Email { get; set; }
        public string Adress { get; set; }
    }
}
