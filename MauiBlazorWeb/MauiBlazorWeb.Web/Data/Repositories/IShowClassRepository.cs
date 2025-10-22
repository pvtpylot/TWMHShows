using System.Collections.Generic;
using System.Threading.Tasks;

namespace MauiBlazorWeb.Web.Data.Repositories
{
    public interface IShowClassRepository : IRepository<ShowClass>
    {
        Task<IEnumerable<ShowClass>> GetAllByShowIdAsync(string showId);
        Task<ShowClass?> GetWithEntriesAsync(string id);
    }
}