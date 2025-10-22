using Microsoft.EntityFrameworkCore;

namespace MauiBlazorWeb.Web.Data.Repositories;

public class DivisionRepository : Repository<Division>, IDivisionRepository
{
    public DivisionRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public Task<Division?> GetByNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return Task.FromResult<Division?>(null);
        return _dbContext.Divisions.FirstOrDefaultAsync(d => d.Name == name);
    }

    public Task<bool> DeleteAsync(int id)
    {
        if (id <= 0) return Task.FromResult(false);

        var division = _dbContext.Divisions.Find(id);
        if (division == null) return Task.FromResult(false);

        // Soft delete implementation
        division.IsDeleted = true;
        division.UpdatedAt = DateTime.UtcNow;
        _dbContext.Entry(division).State = EntityState.Modified;
        return _dbContext.SaveChangesAsync().ContinueWith(t => t.Result > 0);
    }

    public Task<List<Division>> GetAllByShowIdAsync(int showId)
    {
        if (showId > 0)
            return _dbContext.Divisions
                .Where(d => d.ShowId == showId)
                .Include(d => d.ShowClasses)
                .ToListAsync();
        return null;
    }

    public Task<List<Division>> GetAllByShowClassIdAsync(int showClassId)
    {
        if (showClassId > 0)
            return _dbContext.Divisions
                .Where(d => d.Id == showClassId)
                .Include(d => d.ShowClasses)
                .ToListAsync();
        return null;
    }

    public Task<Division?> GetWithResultsAsync(int id)
    {
        if (id <= 0) return Task.FromResult<Division?>(null);
        return _dbContext.Divisions
            .Include(d => d.Show)
            .FirstOrDefaultAsync(d => d.Id == id);
    }
}