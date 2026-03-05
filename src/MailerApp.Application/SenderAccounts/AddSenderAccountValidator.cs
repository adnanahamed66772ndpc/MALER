using FluentValidation;

namespace MailerApp.Application.SenderAccounts;

public class AddSenderAccountValidator : AbstractValidator<AddSenderAccountCommand>
{
    public AddSenderAccountValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.RefreshTokenOrAppPassword)
            .NotEmpty()
            .When(x => x.Provider == MailerApp.Domain.Enums.SenderProvider.GmailSmtp)
            .WithMessage("App password is required for Gmail SMTP.");
        RuleFor(x => x.RefreshTokenOrAppPassword)
            .NotEmpty()
            .When(x => x.Provider == MailerApp.Domain.Enums.SenderProvider.GmailApi)
            .WithMessage("Refresh token is required for Gmail OAuth.");
    }
}
