using RentifyApi.Models;

namespace RentifyApi.Repositories
{
	public interface IPropertyRepository
	{
		Task<List<Property>> GetAllAsync();
		Task<Property?> GetByIdAsync(int id);
		Task<Property> AddAsync(Property property);
		Task UpdateAsync(Property property);
		Task DeleteAsync(int id);
	}
}
