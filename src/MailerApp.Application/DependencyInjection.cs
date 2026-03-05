using FluentValidation;
using MailerApp.Application.Campaigns;
using MailerApp.Application.Contacts;
using MailerApp.Application.Suppression;
using MailerApp.Application.SenderAccounts;
using MailerApp.Application.Templates;
using Microsoft.Extensions.DependencyInjection;

namespace MailerApp.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<AddSenderAccountValidator>();
        services.AddScoped<SenderAccountService>();
        services.AddScoped<ContactImportService>();
        services.AddScoped<ContactListService>();
        services.AddScoped<TemplateService>();
        services.AddScoped<CampaignService>();
        services.AddScoped<SendEngineService>();
        services.AddScoped<CampaignExportService>();
        services.AddScoped<SuppressionListService>();
        return services;
    }
}
