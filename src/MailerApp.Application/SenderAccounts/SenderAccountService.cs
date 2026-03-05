using MailerApp.Domain.Entities;
using MailerApp.Domain.Enums;
using MailerApp.Domain.Interfaces;

namespace MailerApp.Application.SenderAccounts;

public class SenderAccountService
{
    private readonly ISenderAccountRepository _accounts;
    private readonly ITokenEncryption _encryption;
    private readonly IGmailOAuthService _gmailOAuth;

    public SenderAccountService(
        ISenderAccountRepository accounts,
        ITokenEncryption encryption,
        IGmailOAuthService gmailOAuth)
    {
        _accounts = accounts;
        _encryption = encryption;
        _gmailOAuth = gmailOAuth;
    }

    public async Task<IReadOnlyList<SenderAccountDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        var list = await _accounts.GetAllAsync(cancellationToken);
        return list.Select(a => new SenderAccountDto(
            a.Id,
            a.Provider,
            a.Name,
            a.Email,
            a.IsActive,
            a.CreatedAt
        )).ToList();
    }

    public async Task<SenderAccountDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var a = await _accounts.GetByIdAsync(id, cancellationToken);
        return a == null ? null : new SenderAccountDto(a.Id, a.Provider, a.Name, a.Email, a.IsActive, a.CreatedAt);
    }

    /// <summary>Add account with OAuth (encrypted refresh token) or SMTP (encrypted app password).</summary>
    public async Task<SenderAccountDto> AddAsync(AddSenderAccountCommand command, CancellationToken cancellationToken = default)
    {
        var account = new SenderAccount
        {
            Provider = command.Provider,
            Name = command.Name,
            Email = command.Email,
            IsActive = true
        };
        if (command.Provider == SenderProvider.GmailApi && !string.IsNullOrEmpty(command.RefreshTokenOrAppPassword))
            account.EncryptedRefreshToken = _encryption.Encrypt(command.RefreshTokenOrAppPassword);
        else if (command.Provider == SenderProvider.GmailSmtp && !string.IsNullOrEmpty(command.RefreshTokenOrAppPassword))
            account.EncryptedAppPassword = _encryption.Encrypt(command.RefreshTokenOrAppPassword);
        var added = await _accounts.AddAsync(account, cancellationToken);
        return new SenderAccountDto(added.Id, added.Provider, added.Name, added.Email, added.IsActive, added.CreatedAt);
    }

    /// <summary>Complete OAuth flow: exchange code for tokens and add account.</summary>
    public async Task<SenderAccountDto> AddGmailOAuthAsync(string name, string code, CancellationToken cancellationToken = default)
    {
        var (encryptedRefreshToken, email) = await _gmailOAuth.ExchangeCodeAsync(code, cancellationToken);
        var account = new SenderAccount
        {
            Provider = SenderProvider.GmailApi,
            Name = name,
            Email = email,
            EncryptedRefreshToken = encryptedRefreshToken,
            IsActive = true
        };
        var added = await _accounts.AddAsync(account, cancellationToken);
        return new SenderAccountDto(added.Id, added.Provider, added.Name, added.Email, added.IsActive, added.CreatedAt);
    }

    public async Task RemoveAsync(int id, CancellationToken cancellationToken = default)
    {
        await _accounts.DeleteAsync(id, cancellationToken);
    }

    public string GetGmailOAuthAuthorizationUrl(string? state = null) => _gmailOAuth.GetAuthorizationUrl(state);
}
