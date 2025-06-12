using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MauiBlazorWeb.Web.Data.Repositories
{
    public class ShowRepository : Repository<Show>, IShowRepository
    {
        public ShowRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<Show>> GetAllByJudgeIdAsync(string judgeId)
        {
            return await _dbContext.Shows
                .Where(s => s.JudgeId == judgeId)
                .ToListAsync();
        }

        public async Task<Show?> GetByNameAsync(string name)
        {
            return await _dbContext.Shows
                .FirstOrDefaultAsync(s => s.Name == name);
        }

        public async Task<Show?> GetWithClassesAsync(string id)
        {
            if (!int.TryParse(id, out var showId))
                return null;
                
            return await _dbContext.Shows
                .Include(s => s.Classes)
                .FirstOrDefaultAsync(s => s.Id == showId);
        }
    }
}