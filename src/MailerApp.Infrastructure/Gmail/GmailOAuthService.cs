using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Gmail.v1;
using MailerApp.Domain.Interfaces;
using Microsoft.Extensions.Options;

namespace MailerApp.Infrastructure.Gmail;

public class GmailOAuthService : IGmailOAuthService
{
    private readonly GmailOAuthOptions _options;
    private readonly ITokenEncryption _encryption;
    private static readonly string[] Scopes = [GmailService.Scope.MailGoogleCom];

    public GmailOAuthService(IOptions<GmailOAuthOptions> options, ITokenEncryption encryption)
    {
        _options = options.Value;
        _encryption = encryption;
    }

    public string GetAuthorizationUrl(string? state = null)
    {
        if (string.IsNullOrWhiteSpace(_options.ClientId) || string.IsNullOrWhiteSpace(_options.ClientSecret))
            throw new InvalidOperationException(
                "Gmail OAuth is not configured. Add your Google OAuth Client ID and Client Secret to appsettings.json (GmailOAuth section). " +
                "Create credentials at: https://console.cloud.google.com/apis/credentials (Create OAuth 2.0 Client ID, type Desktop or Web, redirect URI: http://localhost:8080/).");
        var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = new ClientSecrets
            {
                ClientId = _options.ClientId,
                ClientSecret = _options.ClientSecret
            },
            Scopes = Scopes
        });
        var request = flow.CreateAuthorizationCodeRequest(_options.RedirectUri);
        request.State = state ?? Guid.NewGuid().ToString("N");
        return request.Build().ToString();
    }

    /// <summary>Exchange authorization code for tokens. Returns refresh token (encrypted) and email for the account.</summary>
    public async Task<(string EncryptedRefreshToken, string Email)> ExchangeCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = new ClientSecrets
            {
                ClientId = _options.ClientId,
                ClientSecret = _options.ClientSecret
            },
            Scopes = Scopes
        });
        var token = await flow.ExchangeCodeForTokenAsync("user", code, _options.RedirectUri, cancellationToken);
        if (string.IsNullOrEmpty(token.RefreshToken))
            throw new InvalidOperationException("No refresh token returned. Ensure the app is not in testing mode or re-authorize with prompt=consent.");
        var credential = new UserCredential(flow, "user", token);
        var service = new GmailService(new Google.Apis.Services.BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "MailerApp"
        });
        var profile = await service.Users.GetProfile("me").ExecuteAsync(cancellationToken);
        var email = profile.EmailAddress ?? "";
        var encrypted = _encryption.Encrypt(token.RefreshToken);
        return (encrypted, email);
    }

    /// <summary>Create a Gmail API credential from stored encrypted refresh token.</summary>
    public async Task<UserCredential> CreateCredentialFromRefreshTokenAsync(string encryptedRefreshToken, CancellationToken cancellationToken = default)
    {
        var refreshToken = _encryption.Decrypt(encryptedRefreshToken);
        var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = new ClientSecrets
            {
                ClientId = _options.ClientId,
                ClientSecret = _options.ClientSecret
            },
            Scopes = Scopes
        });
        var tokenResponse = new TokenResponse { RefreshToken = refreshToken };
        var credential = new UserCredential(flow, "user", tokenResponse);
        return credential;
    }
}
