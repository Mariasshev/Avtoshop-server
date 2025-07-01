using Data_Transfer_Object.DTO.CarDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface ICarService
    {
        Task<CarResponseDto> AddCarWithPhotosAsync(CarCreateDto dto, int userId);
    }
}
