using MailerApp.Domain.Interfaces;
using MailerApp.Infrastructure.Data;
using MailerApp.Infrastructure.Gmail;
using MailerApp.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MailerApp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = DatabasePathHelper.GetConnectionString();
        services.AddDbContext<MailerDbContext>(options => options.UseSqlite(connectionString));

        services.Configure<GmailOAuthOptions>(configuration.GetSection(GmailOAuthOptions.SectionName));

        services.AddScoped<ITokenEncryption, DpapiTokenEncryption>();
        services.AddScoped<GmailOAuthService>();
        services.AddScoped<IGmailOAuthService>(sp => sp.GetRequiredService<GmailOAuthService>());
        services.AddScoped<IEmailSenderFactory, EmailSenderFactory>();

        services.AddScoped<ISenderAccountRepository, SenderAccountRepository>();
        services.AddScoped<IContactRepository, ContactRepository>();
        services.AddScoped<IContactListRepository, ContactListRepository>();
        services.AddScoped<ICampaignRepository, CampaignRepository>();
        services.AddScoped<ICampaignRecipientRepository, CampaignRecipientRepository>();
        services.AddScoped<ITemplateRepository, TemplateRepository>();
        services.AddScoped<ISendJobRepository, SendJobRepository>();
        services.AddScoped<ISendEventRepository, SendEventRepository>();
        services.AddScoped<ISuppressionListRepository, SuppressionListRepository>();

        return services;
    }
}
