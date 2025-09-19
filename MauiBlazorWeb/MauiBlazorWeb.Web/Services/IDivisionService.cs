using MauiBlazorWeb.Web.Data;

namespace MauiBlazorWeb.Web.Services;

public interface IDivisionService
{
    public Task<IEnumerable<Division>> GetAllAsync();
}