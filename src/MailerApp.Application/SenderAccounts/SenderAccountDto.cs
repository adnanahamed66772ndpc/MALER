using MailerApp.Domain.Enums;

namespace MailerApp.Application.SenderAccounts;

public record SenderAccountDto(
    int Id,
    SenderProvider Provider,
    string Name,
    string Email,
    bool IsActive,
    DateTime CreatedAt
);
