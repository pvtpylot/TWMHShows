using System;
using System.Diagnostics;

namespace MauiBlazorWeb.Services
{
    public class DefaultErrorHandler : IErrorHandler
    {
        public void HandleError(Exception ex, string message)
        {
            Debug.WriteLine($"{message}");
            
            if (ex.InnerException != null)
            {
                Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
            
            // In a real app, you might log to a file, service, etc.
            // You could also notify the user via UI
        }

        public void HandleError(Exception ex)
        {
            HandleError(ex, $"Error: {ex.Message}");
        }
    }
}