using System;
using System.Threading.Tasks;
using MauiBlazorWeb.Shared.Models.DTOs;
using MauiBlazorWeb.Shared.Services;
using MauiBlazorWeb.Web.Data;
using MauiBlazorWeb.Web.Data.Repositories;

namespace MauiBlazorWeb.Web.Services
{
    public class ResultService : IResultService
    {
        private readonly IResultRepository _resultRepository;

        public ResultService(IResultRepository resultRepository)
        {
            _resultRepository = resultRepository;
        }

        public async Task<ResultDto?> GetResultByEntryIdAsync(string entryId)
        {
            var result = await _resultRepository.GetByEntryIdAsync(entryId);
            return result != null ? MapToDto(result) : null;
        }

        public async Task<ResultDto?> GetResultByIdAsync(string id)
        {
            var result = await _resultRepository.GetByIdAsync(id);
            return result != null ? MapToDto(result) : null;
        }

        public async Task<ResultDto> CreateResultAsync(ResultDto resultDto)
        {
            if (!int.TryParse(resultDto.EntryId, out var entryId))
                throw new ArgumentException("Invalid entry ID");

            var result = new Result
            {
                Placement = resultDto.Placement,
                Comments = resultDto.Comments,
                JudgedDate = resultDto.JudgedDate,
                EntryId = entryId
            };

            var createdResult = await _resultRepository.CreateAsync(result);
            return MapToDto(createdResult);
        }

        public async Task<ResultDto> UpdateResultAsync(ResultDto resultDto)
        {
            if (!int.TryParse(resultDto.Id, out var resultId))
                throw new ArgumentException("Invalid result ID");
                
            if (!int.TryParse(resultDto.EntryId, out var entryId))
                throw new ArgumentException("Invalid entry ID");

            var result = await _resultRepository.GetByIdAsync(resultDto.Id);
            if (result == null)
                throw new KeyNotFoundException($"Result with ID {resultDto.Id} not found");

            result.Placement = resultDto.Placement;
            result.Comments = resultDto.Comments;
            result.JudgedDate = resultDto.JudgedDate;
            result.EntryId = entryId;

            var updatedResult = await _resultRepository.UpdateAsync(result);
            return MapToDto(updatedResult);
        }

        public async Task<bool> DeleteResultAsync(string id)
        {
            return await _resultRepository.DeleteAsync(id);
        }

        private static ResultDto MapToDto(Result result)
        {
            return new ResultDto
            {
                Id = result.Id.ToString(),
                Placement = result.Placement,
                Comments = result.Comments,
                JudgedDate = result.JudgedDate,
                EntryId = result.EntryId.ToString(),
                CreatedAt = result.CreatedAt,
                UpdatedAt = result.UpdatedAt
            };
        }
    }
}