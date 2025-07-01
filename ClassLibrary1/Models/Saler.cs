using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access.Models
{
    public class Saler
    {
        public Saler()
        {
            this.Cars = new HashSet<Car>();
        }
        public int Id { get; set; }

        public int? UserId { get; set; }           // связь с пользователем (можно nullable, если нужен)

        public string Name { get; set; }
        public string Photo { get; set; }
        public string Number { get; set; }
        public string Email { get; set; }
        public string Adress { get; set; }
        public virtual ICollection<Car> Cars { get; set; }

        // Навигационное свойство к пользователю, если есть модель User
        public virtual User User { get; set; }
    }

}
