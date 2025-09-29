namespace MauiBlazorWeb.Shared.Contracts.Auth.V1;

/// <summary>
/// Contract sent by a client to request a login token pair.
/// </summary>
public sealed record LoginRequest(
    string Email,
    string Password);
