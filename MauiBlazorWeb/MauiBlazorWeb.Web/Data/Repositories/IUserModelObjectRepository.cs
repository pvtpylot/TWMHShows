using MauiBlazorWeb.Web.Data.DTOs;

namespace MauiBlazorWeb.Web.Data.Repositories
{
    public interface IUserModelObjectRepository : IRepository<UserModelObject>
    {
        Task<IEnumerable<UserModelObject>> GetAllAsync(string? applicationUserId);
        Task<UserModelObject?> GetByNameAsync(string name);
    }
}
