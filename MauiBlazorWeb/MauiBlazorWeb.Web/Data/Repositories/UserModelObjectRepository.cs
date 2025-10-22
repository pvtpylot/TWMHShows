using Microsoft.EntityFrameworkCore;

namespace MauiBlazorWeb.Web.Data.Repositories;

public class UserModelObjectRepository : Repository<UserModelObject>, IUserModelObjectRepository
{
    public UserModelObjectRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<UserModelObject>> GetAllAsync(string? applicationUserId)
    {
        return await _dbContext.UserModelObjects
            .Where(u => u.ApplicationUserId == applicationUserId)
            .ToListAsync();
    }

    public async Task<UserModelObject?> GetByNameAsync(string name)
    {
        return await _dbContext.UserModelObjects
            .FirstOrDefaultAsync(u => u.Name == name);
    }
}