using RentifyApi.DTOs;

namespace RentifyApi.Services
{
    public interface IPropertyService
    {
        Task<List<PropertyDto>> GetAllAsync();
        Task<PropertyDto?> GetByIdAsync(int id);
        Task<PropertyDto> CreateAsync(PropertyDto dto);
    }
}
