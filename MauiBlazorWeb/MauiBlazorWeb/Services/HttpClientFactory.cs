using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using MauiBlazorWeb.Models;

namespace MauiBlazorWeb.Services
{
    public class HttpClientFactory : IHttpClientFactory
    {
        private readonly HttpClient _httpClient;
        private readonly MauiAuthenticationStateProvider _authStateProvider;

        public HttpClientFactory(HttpClient httpClient, MauiAuthenticationStateProvider authStateProvider)
        {
            _httpClient = httpClient;
            _authStateProvider = authStateProvider;
        }

        public async Task<HttpClient> CreateAuthenticatedClientAsync()
        {
            var accessTokenInfo = await _authStateProvider.GetAccessTokenInfoAsync();
            if (accessTokenInfo == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated. Please log in.");
            }

            var token = accessTokenInfo.LoginResponse.AccessToken;
            var scheme = accessTokenInfo.LoginResponse.TokenType;

            // Create a new instance to avoid race conditions with headers
            var client = CreateClient();
            client.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue(scheme, token);

            return client;
        }

        public HttpClient CreateClient()
        {
            // Use the HttpClientHelper to get a properly configured HttpClient
            // or return a clone of the injected client
            return HttpClientHelper.GetHttpClient();
        }
    }
}