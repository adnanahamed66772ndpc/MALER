using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using MailerApp.Application.SenderAccounts;
using MailerApp.Domain.Enums;

namespace MailerApp.Desktop.ViewModels;

public class AccountsViewModel : ViewModelBase
{
    private readonly SenderAccountService _senderAccountService;
    private string? _newSmtpName;
    private string? _newSmtpEmail;
    private string? _newSmtpPassword;

    public AccountsViewModel(SenderAccountService senderAccountService, IServiceProvider serviceProvider)
    {
        _senderAccountService = senderAccountService;
        Accounts = new ObservableCollection<SenderAccountDto>();
        AddGmailOAuthCommand = new RelayCommand(_ => AddGmailOAuth());
        AddGmailSmtpCommand = new RelayCommand(_ => AddGmailSmtp());
        RemoveAccountCommand = new RelayCommand(RemoveAccount);
        LoadCommand = new RelayCommand(_ => LoadAsync());
    }

    public ObservableCollection<SenderAccountDto> Accounts { get; }
    public ICommand AddGmailOAuthCommand { get; }
    public ICommand AddGmailSmtpCommand { get; }
    public ICommand RemoveAccountCommand { get; }
    public ICommand LoadCommand { get; }

    public string? NewSmtpName { get => _newSmtpName; set => SetField(ref _newSmtpName, value); }
    public string? NewSmtpEmail { get => _newSmtpEmail; set => SetField(ref _newSmtpEmail, value); }
    public string? NewSmtpPassword { get => _newSmtpPassword; set => SetField(ref _newSmtpPassword, value); }

    private async void LoadAsync()
    {
        var list = await _senderAccountService.ListAsync();
        Accounts.Clear();
        foreach (var a in list)
            Accounts.Add(a);
    }

    private void AddGmailOAuth()
    {
        try
        {
            var url = _senderAccountService.GetGmailOAuthAuthorizationUrl();
            try { Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true }); }
            catch { }
            MessageBox.Show(
                "Complete sign-in in the browser. After you allow access, you may need to copy the redirect URL or code if the app does not receive it automatically.",
                "Gmail OAuth",
                MessageBoxButton.OK);
        }
        catch (InvalidOperationException ex)
        {
            MessageBox.Show(ex.Message, "Gmail OAuth not configured", MessageBoxButton.OK);
        }
    }

    private void AddGmailSmtp()
    {
        if (string.IsNullOrWhiteSpace(NewSmtpName) || string.IsNullOrWhiteSpace(NewSmtpEmail) || string.IsNullOrWhiteSpace(NewSmtpPassword))
        {
            MessageBox.Show("Please enter name, email, and app password.", "Validation", MessageBoxButton.OK);
            return;
        }
        _ = AddSmtpAsync();
    }

    private async System.Threading.Tasks.Task AddSmtpAsync()
    {
        try
        {
            var cmd = new AddSenderAccountCommand(
                SenderProvider.GmailSmtp,
                NewSmtpName!,
                NewSmtpEmail!,
                NewSmtpPassword);
            await _senderAccountService.AddAsync(cmd);
            LoadAsync();
            NewSmtpName = null; NewSmtpEmail = null; NewSmtpPassword = null;
            MessageBox.Show("Account added.", "Done", MessageBoxButton.OK);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
        }
    }

    private async void RemoveAccount(object? parameter)
    {
        if (parameter is not SenderAccountDto dto) return;
        try
        {
            await _senderAccountService.RemoveAsync(dto.Id);
            Accounts.Remove(dto);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
        }
    }
}
