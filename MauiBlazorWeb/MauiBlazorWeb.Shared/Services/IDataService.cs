using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiBlazorWeb.Shared.Models.DTOs;

namespace MauiBlazorWeb.Shared.Services
{
    public interface IDataService
    {
        Task<IEnumerable<UserModelObjectDto>> GetAllUserModelObjectsAsync(string? applicationUserId);
        Task<UserModelObjectDto> GetUserModelObjectByIdAsync(string id);
    }
}
