namespace MailerApp.Desktop.ViewModels;

/// <summary>One row in the "senders with caps" list for a campaign (e.g. sender1: 250, sender2: 200).</summary>
public class SenderCapRow : ViewModelBase
{
    private int _senderAccountId;
    private int _maxEmails = 100;
    private string _displayEmail = "";

    public int SenderAccountId { get => _senderAccountId; set => SetField(ref _senderAccountId, value); }
    public int MaxEmails { get => _maxEmails; set => SetField(ref _maxEmails, value); }
    public string DisplayEmail { get => _displayEmail; set => SetField(ref _displayEmail, value); }
}
