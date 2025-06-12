using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MauiBlazorWeb.Web.Data.Repositories
{
    public class EntryRepository : Repository<Entry>, IEntryRepository
    {
        public EntryRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<Entry>> GetAllByShowClassIdAsync(string showClassId)
        {
            if (int.TryParse(showClassId, out var id))
            {
                return await _dbContext.Entries
                    .Where(e => e.ShowClassId == id)
                    .Include(e => e.UserModelObject)
                    .ToListAsync();
            }
            return new List<Entry>();
        }

        public async Task<IEnumerable<Entry>> GetAllByUserModelObjectIdAsync(string userModelObjectId)
        {
            if (int.TryParse(userModelObjectId, out var id))
            {
                return await _dbContext.Entries
                    .Where(e => e.UserModelObjectId == id)
                    .Include(e => e.ShowClass)
                    .ToListAsync();
            }
            return new List<Entry>();
        }

        public async Task<Entry?> GetWithResultAsync(string id)
        {
            if (int.TryParse(id, out var entryId))
            {
                return await _dbContext.Entries
                    .Include(e => e.Result)
                    .FirstOrDefaultAsync(e => e.Id == entryId);
            }
            return null;
        }
    }
}