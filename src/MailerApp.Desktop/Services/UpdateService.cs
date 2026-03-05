using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace MailerApp.Desktop.Services;

/// <summary>Checks for updates from a JSON feed and downloads the installer for in-place upgrade.</summary>
public class UpdateService
{
    private readonly IConfiguration _config;
    private static readonly HttpClient HttpClient = new() { Timeout = TimeSpan.FromSeconds(15) };

    public UpdateService(IConfiguration config)
    {
        _config = config;
    }

    /// <summary>Current app version (e.g. 1.0.0). Uses InformationalVersion from csproj, then Assembly version.</summary>
    public static string CurrentVersion
    {
        get
        {
            var asm = Assembly.GetExecutingAssembly();
            var info = asm.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            if (!string.IsNullOrWhiteSpace(info?.InformationalVersion))
                return info.InformationalVersion.Split('-')[0].Trim();
            var v = asm.GetName().Version;
            if (v != null) return $"{v.Major}.{v.Minor}.{v.Build}";
            return "1.0.0";
        }
    }

    /// <summary>Check the configured feed URL for a newer version. Feed JSON: { "version": "1.2.0", "downloadUrl": "https://...", "releaseNotes": "..." }.</summary>
    public async Task<UpdateCheckResult> CheckForUpdatesAsync(CancellationToken cancellationToken = default)
    {
        var feedUrl = _config["Update:FeedUrl"]?.Trim();
        if (string.IsNullOrEmpty(feedUrl))
            return new UpdateCheckResult { ErrorMessage = "Update feed URL is not configured (Update:FeedUrl in appsettings.json)." };

        try
        {
            var response = await HttpClient.GetStringAsync(feedUrl, cancellationToken);
            var feed = JsonSerializer.Deserialize<UpdateFeed>(response);
            if (feed?.Version == null || string.IsNullOrWhiteSpace(feed.DownloadUrl))
                return new UpdateCheckResult { ErrorMessage = "Invalid update feed format." };

            var current = ParseVersion(CurrentVersion);
            var latest = ParseVersion(feed.Version.Trim());
            if (latest == null || current >= latest)
                return new UpdateCheckResult { HasUpdate = false };

            return new UpdateCheckResult
            {
                HasUpdate = true,
                NewVersion = feed.Version.Trim(),
                DownloadUrl = feed.DownloadUrl.Trim(),
                ReleaseNotes = feed.ReleaseNotes?.Trim()
            };
        }
        catch (HttpRequestException ex)
        {
            return new UpdateCheckResult { ErrorMessage = "Could not reach update server: " + ex.Message };
        }
        catch (TaskCanceledException)
        {
            return new UpdateCheckResult { ErrorMessage = "Update check timed out." };
        }
        catch (Exception ex)
        {
            return new UpdateCheckResult { ErrorMessage = ex.Message };
        }
    }

    /// <summary>Download the installer from the given URL to a temp file and return the path. Caller should run it and then exit the app.</summary>
    public async Task<string> DownloadInstallerAsync(string downloadUrl, IProgress<double>? progress, CancellationToken cancellationToken = default)
    {
        var fileName = Path.GetFileName(new Uri(downloadUrl).LocalPath);
        if (string.IsNullOrEmpty(fileName)) fileName = "MailerApp-Update.exe";
        var tempPath = Path.Combine(Path.GetTempPath(), "MailerAppUpdate", fileName);
        Directory.CreateDirectory(Path.GetDirectoryName(tempPath)!);

        using var response = await HttpClient.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();
        var total = response.Content.Headers.ContentLength ?? 0L;
        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        await using var file = File.Create(tempPath);
        var buffer = new byte[81920];
        long read = 0;
        int count;
        while ((count = await stream.ReadAsync(buffer, cancellationToken)) > 0)
        {
            await file.WriteAsync(buffer.AsMemory(0, count), cancellationToken);
            read += count;
            if (total > 0) progress?.Report((double)read / total);
        }
        return tempPath;
    }

    /// <summary>Start the installer and return. App should exit after calling this so the installer can replace files. Uses /VERYSILENT for Inno Setup so the upgrade runs without showing the installer UI.</summary>
    public static void RunInstallerAndExit(string installerPath)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = installerPath,
            UseShellExecute = true,
            Arguments = "/VERYSILENT"
        };
        Process.Start(startInfo);
    }

    private static Version? ParseVersion(string? s)
    {
        if (string.IsNullOrWhiteSpace(s)) return null;
        return Version.TryParse(s.Trim(), out var v) ? v : null;
    }

    private sealed class UpdateFeed
    {
        public string? Version { get; set; }
        public string? DownloadUrl { get; set; }
        public string? ReleaseNotes { get; set; }
    }
}
