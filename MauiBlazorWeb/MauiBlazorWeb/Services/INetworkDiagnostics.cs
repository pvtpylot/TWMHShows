using System.Threading.Tasks;

namespace MauiBlazorWeb.Services
{
    /// <summary>
    /// Provides functionality for diagnosing network connectivity issues.
    /// </summary>
    public interface INetworkDiagnostics
    {
        /// <summary>
        /// Tests connectivity to the application's backend services.
        /// </summary>
        /// <returns>True if connectivity test was successful.</returns>
        Task<bool> PerformConnectivityTestAsync();
        
        /// <summary>
        /// Gets a detailed diagnostic report of the current network status.
        /// </summary>
        /// <returns>A string containing diagnostic information.</returns>
        Task<string> GenerateDiagnosticReportAsync();
    }
}