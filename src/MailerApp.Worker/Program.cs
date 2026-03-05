using MailerApp.Application;
using MailerApp.Infrastructure;
using MailerApp.Worker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHostedService<SendWorker>();

var host = builder.Build();
host.Run();
