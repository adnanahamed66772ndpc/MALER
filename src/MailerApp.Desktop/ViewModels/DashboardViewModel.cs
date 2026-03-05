using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using MailerApp.Application.Campaigns;
using Microsoft.Win32;

namespace MailerApp.Desktop.ViewModels;

public class DashboardViewModel : ViewModelBase
{
    private readonly CampaignService _campaignService;
    private readonly CampaignExportService _exportService;
    private CampaignDto? _selectedCampaign;

    public DashboardViewModel(CampaignService campaignService, CampaignExportService exportService)
    {
        _campaignService = campaignService;
        _exportService = exportService;
        Campaigns = new ObservableCollection<CampaignDto>();
        LoadCommand = new RelayCommand(_ => LoadAsync());
        ExportCommand = new RelayCommand(_ => ExportAsync());
    }

    public ObservableCollection<CampaignDto> Campaigns { get; }
    public ICommand LoadCommand { get; }
    public ICommand ExportCommand { get; }
    public CampaignDto? SelectedCampaign { get => _selectedCampaign; set => SetField(ref _selectedCampaign, value); }

    public async void LoadAsync()
    {
        var list = await _campaignService.GetAllAsync();
        Campaigns.Clear();
        foreach (var c in list) Campaigns.Add(c);
    }

    private async void ExportAsync()
    {
        if (SelectedCampaign == null) { MessageBox.Show("Select a campaign."); return; }
        var dlg = new SaveFileDialog { Filter = "CSV|*.csv|All|*.*", FileName = $"campaign-{SelectedCampaign.Id}.csv" };
        if (dlg.ShowDialog() != true) return;
        try
        {
            var csv = await _exportService.ExportCampaignRecipientsCsvAsync(SelectedCampaign.Id);
            await File.WriteAllTextAsync(dlg.FileName, csv);
            MessageBox.Show("Exported.");
        }
        catch (Exception ex) { MessageBox.Show(ex.Message); }
    }
}
