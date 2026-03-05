using System.IO;
using System.Windows;
using MailerApp.Application.Campaigns;
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
    private CancellationTokenSource? _sendLoopCts;

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
        StartSendLoopInBackground();
        var mainWindow = new MainWindow(_serviceProvider);
        mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _sendLoopCts?.Cancel();
        base.OnExit(e);
    }

    /// <summary>All-in-one: Desktop চালালেই ব্যাকগ্রাউন্ডে ইমেল সেন্ড লুপ চলে (আলাদা Worker চালানোর দরকার নেই)।</summary>
    private void StartSendLoopInBackground()
    {
        _sendLoopCts = new CancellationTokenSource();
        var token = _sendLoopCts.Token;
        _ = Task.Run(async () =>
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    if (_serviceProvider != null)
                    {
                        using var scope = _serviceProvider.CreateScope();
                        var engine = scope.ServiceProvider.GetRequiredService<SendEngineService>();
                        await engine.ProcessPendingJobsAsync(10, token);
                    }
                }
                catch (OperationCanceledException) { break; }
                catch (Exception ex) { Log.Warning(ex, "Send loop error"); }
                await Task.Delay(TimeSpan.FromSeconds(30), token);
            }
        }, token);
    }
}
