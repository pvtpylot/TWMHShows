using System.Net;
using System.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;

namespace MauiBlazorWeb.Services
{
    public class AuthHeaderHandler : DelegatingHandler
    {
        private readonly MauiAuthenticationStateProvider _auth;

        public AuthHeaderHandler(MauiAuthenticationStateProvider auth) => _auth = auth;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var tokenInfo = await _auth.GetAccessTokenInfoAsync();
            var token = tokenInfo?.LoginResponse?.AccessToken ?? await SecureStorage.Default.GetAsync("access_token");

            if (!string.IsNullOrEmpty(token) && !IsJwtExpired(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                // Clear invalid/expired credentials to avoid "sticky" auth state
                SecureStorage.Default.Remove("access_token");
                SecureStorage.Default.Remove("user_id");
                SecureStorage.Default.Remove("user_roles");
                try { await _auth.Logout(); } catch { /* best effort */ }
            }

            return response;
        }

        private static bool IsJwtExpired(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);
                return jwt.ValidTo <= DateTime.UtcNow.AddSeconds(-60);
            }
            catch
            {
                return true;
            }
        }
    }
}