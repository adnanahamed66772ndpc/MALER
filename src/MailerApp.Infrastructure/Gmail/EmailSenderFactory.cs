using Google.Apis.Gmail.v1;
using MailerApp.Domain.Entities;
using MailerApp.Domain.Enums;
using MailerApp.Domain.Interfaces;

namespace MailerApp.Infrastructure.Gmail;

public class EmailSenderFactory : IEmailSenderFactory
{
    private readonly GmailOAuthService _gmailOAuth;
    private readonly ITokenEncryption _encryption;

    public EmailSenderFactory(GmailOAuthService gmailOAuth, ITokenEncryption encryption)
    {
        _gmailOAuth = gmailOAuth;
        _encryption = encryption;
    }

    public async Task<IEmailSender?> CreateForAccountAsync(SenderAccount account, CancellationToken cancellationToken = default)
    {
        if (account.Provider == SenderProvider.GmailApi)
            return await CreateGmailApiSenderAsync(account, cancellationToken);
        if (account.Provider == SenderProvider.GmailSmtp)
            return CreateGmailSmtpSender(account);
        return null;
    }

    private async Task<IEmailSender?> CreateGmailApiSenderAsync(SenderAccount account, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(account.EncryptedRefreshToken)) return null;
        var credential = await _gmailOAuth.CreateCredentialFromRefreshTokenAsync(account.EncryptedRefreshToken, cancellationToken);
        var service = new GmailService(new Google.Apis.Services.BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "MailerApp"
        });
        return new GmailApiSender(service, account.Email);
    }

    private IEmailSender? CreateGmailSmtpSender(SenderAccount account)
    {
        if (string.IsNullOrEmpty(account.EncryptedAppPassword)) return null;
        var password = _encryption.Decrypt(account.EncryptedAppPassword);
        return new GmailSmtpSender(account.Email, password);
    }
}
