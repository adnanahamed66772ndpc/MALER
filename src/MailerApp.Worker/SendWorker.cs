using MailerApp.Application.Campaigns;
using Microsoft.Extensions.DependencyInjection;

namespace MailerApp.Worker;

public class SendWorker : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<SendWorker> _logger;

    public SendWorker(IServiceProvider services, ILogger<SendWorker> logger)
    {
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Send worker started.");
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _services.CreateScope();
                var engine = scope.ServiceProvider.GetRequiredService<SendEngineService>();
                await engine.ProcessPendingJobsAsync(10, stoppingToken);
            }
            catch (OperationCanceledException) { break; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing send queue.");
            }
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}
