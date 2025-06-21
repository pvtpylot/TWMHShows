using System;

namespace MauiBlazorWeb.Services
{
    /// <summary>
    /// Interface for handling errors consistently across the application
    /// </summary>
    public interface IErrorHandler
    {
        /// <summary>
        /// Handles an exception with a custom message
        /// </summary>
        /// <param name="ex">The exception that occurred</param>
        /// <param name="message">Custom message to log</param>
        void HandleError(Exception ex, string message);
        
        /// <summary>
        /// Handles an exception
        /// </summary>
        /// <param name="ex">The exception that occurred</param>
        void HandleError(Exception ex);
    }
}