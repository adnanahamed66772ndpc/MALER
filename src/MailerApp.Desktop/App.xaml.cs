using System.IO;
using System.Windows;
using MailerApp.Application;
using MailerApp.Infrastructure;
using MailerApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace MailerApp.Desktop;

public partial class App : System.Windows.Application
{
    private IServiceProvider? _serviceProvider;

    protected override void OnStartup(StartupEventArgs e)
    {
        var logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MailerApp", "logs", "desktop-.log");
        Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
            .CreateLogger();
        base.OnStartup(e);
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .Build();
        var services = new ServiceCollection();
        services.AddSingleton<Microsoft.Extensions.Configuration.IConfiguration>(config);
        services.AddSingleton<MailerApp.Desktop.Services.UpdateService>();
        services.AddApplication();
        services.AddInfrastructure(config);
        _serviceProvider = services.BuildServiceProvider();
        using (var scope = _serviceProvider.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<MailerDbContext>();
            db.Database.Migrate();
        }
        var mainWindow = new MainWindow(_serviceProvider);
        mainWindow.Show();
    }
}
