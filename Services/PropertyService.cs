using RentifyApi.DTOs;
using RentifyApi.Models;
using RentifyApi.Repositories;

namespace RentifyApi.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly IPropertyRepository _repo;
        public PropertyService(IPropertyRepository repo) => _repo = repo;

        public async Task<List<PropertyDto>> GetAllAsync()
        {
            var props = await _repo.GetAllAsync();
            return props.Select(p => new PropertyDto {
                Id = p.Id, Title = p.Title, Description = p.Description,
                Address = p.Address, PricePerNight = p.PricePerNight, ImageUrl = p.ImageUrl
            }).ToList();
        }

        public async Task<PropertyDto?> GetByIdAsync(int id)
        {
            var p = await _repo.GetByIdAsync(id);
            if (p == null) return null;
            return new PropertyDto {
                Id = p.Id, Title = p.Title, Description = p.Description,
                Address = p.Address, PricePerNight = p.PricePerNight, ImageUrl = p.ImageUrl
            };
        }

        public async Task<PropertyDto> CreateAsync(PropertyDto dto)
        {
            var p = new Property {
                Title = dto.Title,
                Description = dto.Description,
                Address = dto.Address,
                PricePerNight = dto.PricePerNight,
                ImageUrl = dto.ImageUrl
            };
            var added = await _repo.AddAsync(p);
            dto.Id = added.Id;
            return dto;
        }
    }
}
