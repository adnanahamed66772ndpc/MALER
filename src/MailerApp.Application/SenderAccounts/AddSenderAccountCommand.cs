using MailerApp.Domain.Enums;

namespace MailerApp.Application.SenderAccounts;

public record AddSenderAccountCommand(
    SenderProvider Provider,
    string Name,
    string Email,
    string? RefreshTokenOrAppPassword
);
