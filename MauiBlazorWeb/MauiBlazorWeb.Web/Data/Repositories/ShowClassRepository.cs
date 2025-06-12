using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MauiBlazorWeb.Web.Data.Repositories
{
    public class ShowClassRepository : Repository<ShowClass>, IShowClassRepository
    {
        public ShowClassRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<ShowClass>> GetAllByShowIdAsync(string showId)
        {
            if (int.TryParse(showId, out var id))
            {
                return await _dbContext.ShowClasses
                    .Where(sc => sc.ShowId == id)
                    .ToListAsync();
            }
            return new List<ShowClass>();
        }

        public async Task<ShowClass?> GetWithEntriesAsync(string id)
        {
            if (int.TryParse(id, out var classId))
            {
                return await _dbContext.ShowClasses
                    .Include(sc => sc.Entries)
                    .FirstOrDefaultAsync(sc => sc.Id == classId);
            }
            return null;
        }
    }
}