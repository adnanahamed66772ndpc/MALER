using MailerApp.Domain.Enums;

namespace MailerApp.Domain.Entities;

public class SenderAccount
{
    public int Id { get; set; }
    public SenderProvider Provider { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    /// <summary>Encrypted refresh token (Gmail OAuth) or null for SMTP.</summary>
    public string? EncryptedRefreshToken { get; set; }
    /// <summary>Encrypted app password (Gmail SMTP) or null for OAuth.</summary>
    public string? EncryptedAppPassword { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
