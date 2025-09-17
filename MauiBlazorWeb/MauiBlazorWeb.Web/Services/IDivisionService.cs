using MauiBlazorWeb.Web.Data;

namespace MauiBlazorWeb.Web.Services;

public class IDivisionService
{
    public Task<IEnumerable<Division>> GetAllAsync();
}