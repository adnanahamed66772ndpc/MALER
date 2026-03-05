namespace MailerApp.Domain.Interfaces;

public interface IGmailOAuthService
{
    string GetAuthorizationUrl(string? state = null);
    Task<(string EncryptedRefreshToken, string Email)> ExchangeCodeAsync(string code, CancellationToken cancellationToken = default);
}
