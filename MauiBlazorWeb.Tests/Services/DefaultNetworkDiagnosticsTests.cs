using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MauiBlazorWeb.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Xunit;

namespace MauiBlazorWeb.Tests.Services
{
    public class DefaultNetworkDiagnosticsTests
    {
        [Fact]
        public async Task PerformConnectivityTestAsync_Success_ReturnsTrue()
        {
            // Arrange
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                });

            var httpClient = new HttpClient(handlerMock.Object);

            // Use a test double to provide the HttpClient
            var diagnostics = new TestDefaultNetworkDiagnostics(httpClient);

            // Act
            var result = await diagnostics.PerformConnectivityTestAsync();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task PerformConnectivityTestAsync_Failure_ReturnsFalse()
        {
            // Arrange
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError
                });

            var httpClient = new HttpClient(handlerMock.Object);

            // Use a test double to provide the HttpClient
            var diagnostics = new TestDefaultNetworkDiagnostics(httpClient);

            // Act
            var result = await diagnostics.PerformConnectivityTestAsync();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task PerformConnectivityTestAsync_Exception_ReturnsFalse()
        {
            // Arrange
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Connection failed"));

            var httpClient = new HttpClient(handlerMock.Object);

            // Use a test double to provide the HttpClient
            var diagnostics = new TestDefaultNetworkDiagnostics(httpClient);

            // Act
            var result = await diagnostics.PerformConnectivityTestAsync();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GenerateDiagnosticReportAsync_ReturnsNonEmptyString()
        {
            // Arrange
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                });

            var httpClient = new HttpClient(handlerMock.Object);
            var diagnostics = new TestDefaultNetworkDiagnostics(httpClient);

            // Act
            var result = await diagnostics.GenerateDiagnosticReportAsync();

            // Assert
            Assert.NotEmpty(result);
            Assert.Contains("Platform:", result);
            Assert.Contains("Backend connectivity:", result);
        }

        // Test helper class to override HttpClient creation
        private class TestDefaultNetworkDiagnostics : DefaultNetworkDiagnostics
        {
            private readonly HttpClient _httpClient;

            public TestDefaultNetworkDiagnostics(HttpClient httpClient)
            {
                _httpClient = httpClient;
            }

            protected override HttpClient GetHttpClient()
            {
                return _httpClient;
            }
        }
    }
}