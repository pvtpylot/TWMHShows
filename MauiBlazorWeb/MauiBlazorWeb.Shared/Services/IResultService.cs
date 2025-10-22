using MauiBlazorWeb.Shared.Models.DTOs;

namespace MauiBlazorWeb.Shared.Services;

public interface IResultService
{
    Task<ResultDto?> GetResultByEntryIdAsync(string entryId);
    Task<ResultDto?> GetResultByIdAsync(string id);
    Task<ResultDto> CreateResultAsync(ResultDto resultDto);
    Task<ResultDto> UpdateResultAsync(ResultDto resultDto);
    Task<bool> DeleteResultAsync(string id);
}