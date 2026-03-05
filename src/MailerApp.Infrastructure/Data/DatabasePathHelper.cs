namespace MailerApp.Infrastructure.Data;

public static class DatabasePathHelper
{
    public static string GetDatabasePath()
    {
        var basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MailerApp");
        Directory.CreateDirectory(basePath);
        return Path.Combine(basePath, "mailer.db");
    }

    public static string GetConnectionString() => $"Data Source={GetDatabasePath()}";
}
