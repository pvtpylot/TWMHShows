using Microsoft.EntityFrameworkCore;

namespace MauiBlazorWeb.Web.Data.Repositories;

public class ResultRepository : Repository<Result>, IResultRepository
{
    public ResultRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Result?> GetByEntryIdAsync(string entryId)
    {
        if (int.TryParse(entryId, out var id))
            return await _dbContext.Results
                .FirstOrDefaultAsync(r => r.EntryId == id);
        return null;
    }
}