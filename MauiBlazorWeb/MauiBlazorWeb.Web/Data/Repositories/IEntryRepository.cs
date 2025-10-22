using System.Collections.Generic;
using System.Threading.Tasks;

namespace MauiBlazorWeb.Web.Data.Repositories
{
    public interface IEntryRepository : IRepository<Entry>
    {
        Task<IEnumerable<Entry>> GetAllByShowClassIdAsync(string showClassId);
        Task<IEnumerable<Entry>> GetAllByUserModelObjectIdAsync(string userModelObjectId);
        Task<Entry?> GetWithResultAsync(string id);
    }
}