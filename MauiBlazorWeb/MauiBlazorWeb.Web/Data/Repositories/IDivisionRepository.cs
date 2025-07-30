namespace MauiBlazorWeb.Web.Data.Repositories;

public interface IDivisionRepository : IRepository<Division>
{
    Task<IEnumerable<Division>> GetAllAsync();
    Task<Division?> GetByNameAsync(string name);
    Task<Division?> GetByIdAsync(string id);
    Task<Division?> CreateAsync(Division division);
    Task<Division?> UpdateAsync(Division division);
    Task<bool> DeleteAsync(int id);
    Task<List<Division>> GetAllByShowIdAsync(int showId);
    Task<List<Division>> GetAllByShowClassIdAsync(int showClassId);
    Task<Division?> GetWithResultsAsync(int id);
}