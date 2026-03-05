using MailerApp.Domain.Entities;

namespace MailerApp.Domain.Interfaces;

public interface IEmailSenderFactory
{
    Task<IEmailSender?> CreateForAccountAsync(SenderAccount account, CancellationToken cancellationToken = default);
}
