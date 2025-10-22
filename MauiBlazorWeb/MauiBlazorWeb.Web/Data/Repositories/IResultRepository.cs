namespace MauiBlazorWeb.Web.Data.Repositories;

public interface IResultRepository : IRepository<Result>
{
    Task<Result?> GetByEntryIdAsync(string entryId);
}