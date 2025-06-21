using System.Threading.Tasks;
using MauiBlazorWeb.Models;

namespace MauiBlazorWeb.Services
{
    /// <summary>
    /// Provides functionality for storing, retrieving, and managing authentication tokens.
    /// </summary>
    public interface ITokenStorage
    {
        /// <summary>
        /// Gets the stored access token information if available.
        /// </summary>
        /// <returns>The access token information or null if not found.</returns>
        Task<AccessTokenInfo?> GetTokenAsync();
        
        /// <summary>
        /// Saves an authentication token and associated information.
        /// </summary>
        /// <param name="tokenJson">The JSON representation of the token response.</param>
        /// <param name="email">The user's email address.</param>
        /// <returns>The processed access token information.</returns>
        Task<AccessTokenInfo> SaveTokenAsync(string tokenJson, string email);
        
        /// <summary>
        /// Removes stored authentication tokens.
        /// </summary>
        Task RemoveTokenAsync();
        
        /// <summary>
        /// Checks if there is a valid token stored.
        /// </summary>
        /// <returns>True if a valid non-expired token exists.</returns>
        Task<bool> HasValidTokenAsync();
        
        /// <summary>
        /// Gets the user's email address if it was saved during previous authentication.
        /// </summary>
        /// <returns>The saved email or null if not found.</returns>
        Task<string?> GetSavedEmailAsync();
    }
}