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
        Task<CarResponseDto> UpdateCarWithPhotosAsync(int carId, CarCreateDto dto, int userId, List<string> photosToDelete);
        Task<CarResponseDto> GetCarByIdAsync(int id);
        Task<CarResponseDto> UpdateCarAsync(CarUpdateDto dto);

    }
}
