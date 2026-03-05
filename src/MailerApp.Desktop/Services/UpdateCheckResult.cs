namespace MailerApp.Desktop.Services;

/// <summary>Result of checking for updates (feed returns version, download URL, release notes).</summary>
public sealed class UpdateCheckResult
{
    public bool HasUpdate { get; init; }
    public string? NewVersion { get; init; }
    public string? DownloadUrl { get; init; }
    public string? ReleaseNotes { get; init; }
    public string? ErrorMessage { get; init; }
}
