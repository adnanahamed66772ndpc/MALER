using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using MailerApp.Application.Suppression;
using MailerApp.Desktop.Services;
using MailerApp.Domain.Enums;

namespace MailerApp.Desktop.ViewModels;

public class SettingsViewModel : ViewModelBase
{
    private readonly SuppressionListService _suppressionService;
    private readonly UpdateService _updateService;
    private string? _newSuppressionEmail;
    private SuppressionReason _newSuppressionReason = SuppressionReason.Manual;
    private bool _isCheckingForUpdates;
    private bool _isDownloadingUpdate;
    private string? _updateStatusMessage;
    private UpdateCheckResult? _pendingUpdate;

    public SettingsViewModel(SuppressionListService suppressionService, UpdateService updateService)
    {
        _suppressionService = suppressionService;
        _updateService = updateService;
        SuppressionList = new ObservableCollection<SuppressionEntryDto>();
        SuppressionReasons = new ObservableCollection<SuppressionReason>(Enum.GetValues<SuppressionReason>());
        LoadSuppressionCommand = new RelayCommand(_ => LoadSuppressionAsync());
        AddSuppressionCommand = new RelayCommand(_ => AddSuppressionAsync());
        CheckForUpdatesCommand = new RelayCommand(_ => CheckForUpdatesAsync(), _ => !IsCheckingForUpdates && !IsDownloadingUpdate);
        DownloadAndInstallCommand = new RelayCommand(_ => DownloadAndInstallAsync(), _ => PendingUpdate?.HasUpdate == true && !IsDownloadingUpdate);
        CurrentVersion = UpdateService.CurrentVersion;
    }

    public ObservableCollection<SuppressionReason> SuppressionReasons { get; }

    public ObservableCollection<SuppressionEntryDto> SuppressionList { get; }
    public ICommand LoadSuppressionCommand { get; }
    public ICommand AddSuppressionCommand { get; }
    public ICommand CheckForUpdatesCommand { get; }
    public ICommand DownloadAndInstallCommand { get; }

    public string CurrentVersion { get; }
    public bool IsCheckingForUpdates { get => _isCheckingForUpdates; set { if (SetField(ref _isCheckingForUpdates, value)) CommandManager.InvalidateRequerySuggested(); } }
    public bool IsDownloadingUpdate { get => _isDownloadingUpdate; set { if (SetField(ref _isDownloadingUpdate, value)) CommandManager.InvalidateRequerySuggested(); } }
    public string? UpdateStatusMessage { get => _updateStatusMessage; set => SetField(ref _updateStatusMessage, value); }
    public UpdateCheckResult? PendingUpdate { get => _pendingUpdate; set { if (SetField(ref _pendingUpdate, value)) { CommandManager.InvalidateRequerySuggested(); OnPropertyChanged(nameof(ReleaseNotesText)); } } }
    public string ReleaseNotesText => PendingUpdate?.ReleaseNotes ?? "";

    public string? NewSuppressionEmail { get => _newSuppressionEmail; set => SetField(ref _newSuppressionEmail, value); }
    public SuppressionReason NewSuppressionReason { get => _newSuppressionReason; set => SetField(ref _newSuppressionReason, value); }

    public async void LoadSuppressionAsync()
    {
        var list = await _suppressionService.GetAllAsync(0, 500);
        SuppressionList.Clear();
        foreach (var e in list) SuppressionList.Add(e);
    }

    private async void AddSuppressionAsync()
    {
        if (string.IsNullOrWhiteSpace(NewSuppressionEmail)) { MessageBox.Show("Enter email."); return; }
        try
        {
            await _suppressionService.AddAsync(NewSuppressionEmail.Trim(), NewSuppressionReason, "Manual");
            NewSuppressionEmail = null;
            LoadSuppressionAsync();
            MessageBox.Show("Added to suppression list.");
        }
        catch (Exception ex) { MessageBox.Show(ex.Message); }
    }

    private async void CheckForUpdatesAsync()
    {
        IsCheckingForUpdates = true;
        UpdateStatusMessage = null;
        PendingUpdate = null;
        try
        {
            var result = await _updateService.CheckForUpdatesAsync();
            if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                UpdateStatusMessage = result.ErrorMessage;
                return;
            }
            if (result.HasUpdate)
            {
                PendingUpdate = result;
                UpdateStatusMessage = $"Update available: {result.NewVersion}. Click \"Download and install\" to upgrade (no uninstall needed).";
            }
            else
            {
                UpdateStatusMessage = "You are on the latest version.";
            }
        }
        finally
        {
            IsCheckingForUpdates = false;
        }
    }

    private async void DownloadAndInstallAsync()
    {
        var update = PendingUpdate;
        if (update?.HasUpdate != true || string.IsNullOrEmpty(update.DownloadUrl)) return;
        IsDownloadingUpdate = true;
        UpdateStatusMessage = "Downloading update...";
        try
        {
            var progress = new Progress<double>(p => UpdateStatusMessage = $"Downloading... {p:P0}");
            var installerPath = await _updateService.DownloadInstallerAsync(update.DownloadUrl, progress);
            UpdateStatusMessage = "Starting installer. The app will close; the new version will replace the old one without uninstalling.";
            UpdateService.RunInstallerAndExit(installerPath);
            System.Windows.Application.Current.Shutdown();
        }
        catch (Exception ex)
        {
            UpdateStatusMessage = "Download failed: " + ex.Message;
            IsDownloadingUpdate = false;
        }
    }
}
