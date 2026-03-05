namespace MailerApp.Infrastructure.Gmail;

public class GmailOAuthOptions
{
    public const string SectionName = "GmailOAuth";
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    /// <summary>Redirect URI registered in Google Cloud Console (e.g. http://localhost:8080/ or urn:ietf:wg:oauth:2.0:oob for out-of-band).</summary>
    public string RedirectUri { get; set; } = "http://localhost:8080/";
}
