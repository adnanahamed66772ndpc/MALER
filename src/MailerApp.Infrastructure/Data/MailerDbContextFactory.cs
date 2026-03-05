using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace MailerApp.Infrastructure.Data;

public class MailerDbContextFactory : IDesignTimeDbContextFactory<MailerDbContext>
{
    public MailerDbContext CreateDbContext(string[] args)
    {
        var basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MailerApp");
        Directory.CreateDirectory(basePath);
        var connectionString = $"Data Source={Path.Combine(basePath, "mailer.db")}";

        var optionsBuilder = new DbContextOptionsBuilder<MailerDbContext>();
        optionsBuilder.UseSqlite(connectionString);
        return new MailerDbContext(optionsBuilder.Options);
    }
}
