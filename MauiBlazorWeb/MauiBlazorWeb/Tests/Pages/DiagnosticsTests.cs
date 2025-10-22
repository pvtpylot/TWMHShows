using Bunit;
using FluentAssertions;
using MauiBlazorWeb.Models;
using MauiBlazorWeb.Pages;
using MauiBlazorWeb.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace MauiBlazorWeb.Tests.Pages
{
    public class DiagnosticsTests : TestContext
    {
        private readonly Mock<MauiAuthenticationStateProvider> _authStateProviderMock;
        private readonly Mock<ILogger<Diagnostics>> _loggerMock;

        public DiagnosticsTests()
        {
            _authStateProviderMock = new Mock<MauiAuthenticationStateProvider>();
            _loggerMock = new Mock<ILogger<Diagnostics>>();

            Services.AddSingleton(_authStateProviderMock.Object);
            Services.AddSingleton(_loggerMock.Object);
        }

        [Fact]
        public void Diagnostics_InitialRender_ShowsRunButton()
        {
            // Arrange
            JSInterop.Mode = JSRuntimeMode.Loose;

            // Act
            var cut = RenderComponent<Diagnostics>();

            // Assert
            cut.Find("button").TextContent.Should().Contain("Run Diagnostics");
        }

        [Fact]
        public async Task RunDiagnostics_ShouldDisplayProgressBar()
        {
            // Arrange
            _authStateProviderMock
                .Setup(a => a.GetAuthenticationStateAsync())
                .ReturnsAsync(CreateAuthenticationState(false));

            JSInterop.Mode = JSRuntimeMode.Loose;
            var cut = RenderComponent<Diagnostics>();
            
            // Act - Start but don't wait for completion
            var runTask = cut.Instance.RunDiagnostics();
            
            // Render changes without waiting for task completion
            cut.Render();
            
            // Assert
            cut.Find(".progress").Should().NotBeNull();
            cut.Find(".alert-info").TextContent.Should().Contain("Running diagnostics");
            
            // Let the task complete
            await runTask;
        }

        [Fact]
        public async Task RunDiagnostics_DisplaysResults()
        {
            // Arrange
            _authStateProviderMock
                .Setup(a => a.GetAuthenticationStateAsync())
                .ReturnsAsync(CreateAuthenticationState(true));
            
            _authStateProviderMock
                .Setup(a => a.GetAccessTokenInfoAsync())
                .ReturnsAsync(new AccessTokenInfo
                {
                    Email = "test@example.com",
                    LoginResponse = new LoginResponse
                    {
                        TokenType = "Bearer",
                        AccessToken = "dummy-access-token",
                        ExpiresIn = 3600,
                        RefreshToken = "dummy-refresh-token"
                    },
                    AccessTokenExpiration = DateTime.UtcNow.AddHours(1)
                });

            JSInterop.Mode = JSRuntimeMode.Loose;
            var cut = RenderComponent<Diagnostics>();
            
            // Act
            await cut.Instance.RunDiagnostics();
            cut.Render();
            
            // Assert
            cut.FindAll(".list-group-item").Count.Should().BeGreaterThan(0);
            cut.Find("h4").TextContent.Should().Contain("Results");
        }

        private AuthenticationState CreateAuthenticationState(bool isAuthenticated)
        {
            var identity = new ClaimsIdentity(
                isAuthenticated ? 
                    new List<Claim> { new Claim(ClaimTypes.Name, "TestUser") } : 
                    new List<Claim>(),
                "test",
                ClaimTypes.Name,
                ClaimTypes.Role);

            var user = new ClaimsPrincipal(identity);
            return new AuthenticationState(user);
        }
    }
}