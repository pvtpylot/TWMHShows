using System.Collections.Generic;
using System.Threading.Tasks;

namespace MauiBlazorWeb.Web.Data.Repositories
{
    public interface IShowRepository : IRepository<Show>
    {
        Task<IEnumerable<Show>> GetAllByJudgeIdAsync(string judgeId);
        Task<Show?> GetByNameAsync(string name);
        Task<Show?> GetWithClassesAsync(string id);

        // Added for ShowHolder flows
        Task<IEnumerable<Show>> GetAllByShowHolderUserIdAsync(string ownerUserId);
        Task<Show?> GetByIdWithShowHolderAsync(string id);
    }
}