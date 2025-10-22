using MauiBlazorWeb.Services;
using Xunit;

namespace MauiBlazorWeb.Tests.Services;

public class NetworkDiagnosticsTests
{
    [Fact]
    public async Task RunNetworkDiagnosticsAsync_ReturnsNonEmptyString()
    {
        // Using static method tests requires a different approach
        // This test might need to be integration test instead of unit test
        var result = await NetworkDiagnostics.RunNetworkDiagnosticsAsync();

        // Just verify we get some kind of result
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Contains("Platform:", result);
    }

    [Fact]
    public async Task TestLoginEndpoint_ReturnsNonEmptyString()
    {
        // Using static method tests requires a different approach
        // This test might need to be integration test instead of unit test
        var result = await NetworkDiagnostics.TestLoginEndpoint();

        // Just verify we get some kind of result
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Contains("Testing login endpoint:", result);
    }
}