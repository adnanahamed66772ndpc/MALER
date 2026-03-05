namespace MailerApp.Domain.Entities;

public class Contact
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Company { get; set; }
    /// <summary>JSON object for custom key-value fields.</summary>
    public string? CustomFieldsJson { get; set; }
    public DateTime CreatedAt { get; set; }
}
