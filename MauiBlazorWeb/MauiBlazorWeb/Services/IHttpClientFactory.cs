namespace MauiBlazorWeb.Services;

/// <summary>
///     Factory for creating and configuring HttpClient instances
/// </summary>
public interface IHttpClientFactory
{
    /// <summary>
    ///     Creates an HttpClient with authentication headers
    /// </summary>
    /// <returns>An authenticated HttpClient</returns>
    Task<HttpClient> CreateAuthenticatedClientAsync();

    /// <summary>
    ///     Creates a standard HttpClient without authentication
    /// </summary>
    /// <returns>An HttpClient without authentication</returns>
    HttpClient CreateClient();
}