using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MailerApp.Application.Campaigns;
using MailerApp.Application.Contacts;
using MailerApp.Application.SenderAccounts;
using MailerApp.Application.Templates;

namespace MailerApp.Desktop.ViewModels;

public class CampaignsViewModel : ViewModelBase
{
    private readonly CampaignService _campaignService;
    private readonly ContactListService _listService;
    private CampaignDto? _selectedCampaign;
    private string _newName = "";
    private int _selectedTemplateId;
    private int _selectedSenderId;
    private int _dailyCap = 100;
    private int _delayMin = 2000;
    private int _delayMax = 5000;
    private int _selectedListId;

    public CampaignsViewModel(
        CampaignService campaignService,
        ContactListService listService,
        SenderAccountService senderService,
        TemplateService templateService)
    {
        _campaignService = campaignService;
        _listService = listService;
        Campaigns = new ObservableCollection<CampaignDto>();
        Templates = new ObservableCollection<TemplateDto>();
        Senders = new ObservableCollection<SenderAccountDto>();
        Lists = new ObservableCollection<ContactListDto>();
        SenderCaps = new ObservableCollection<SenderCapRow>();
        LoadCommand = new RelayCommand(_ => LoadAsync());
        AddSenderCapCommand = new RelayCommand(_ => AddSenderCap());
        RemoveSenderCapCommand = new RelayCommand(RemoveSenderCap);
        CreateCommand = new RelayCommand(_ => CreateAsync());
        StartCommand = new RelayCommand(_ => StartAsync());
        PauseCommand = new RelayCommand(_ => PauseAsync());
        StopCommand = new RelayCommand(_ => StopAsync());
        RefreshCommand = new RelayCommand(_ => LoadAsync());
        LoadTemplatesAndSendersAsync(senderService, templateService);
    }

    public ObservableCollection<CampaignDto> Campaigns { get; }
    public ObservableCollection<TemplateDto> Templates { get; }
    public ObservableCollection<SenderAccountDto> Senders { get; }
    public ObservableCollection<ContactListDto> Lists { get; }
    public ObservableCollection<SenderCapRow> SenderCaps { get; }
    public ICommand LoadCommand { get; }
    public ICommand AddSenderCapCommand { get; }
    public ICommand RemoveSenderCapCommand { get; }
    public ICommand CreateCommand { get; }
    public ICommand StartCommand { get; }
    public ICommand PauseCommand { get; }
    public ICommand StopCommand { get; }
    public ICommand RefreshCommand { get; }

    public CampaignDto? SelectedCampaign { get => _selectedCampaign; set => SetField(ref _selectedCampaign, value); }
    public string NewName { get => _newName; set => SetField(ref _newName, value); }
    public int SelectedTemplateId { get => _selectedTemplateId; set => SetField(ref _selectedTemplateId, value); }
    public int SelectedSenderId { get => _selectedSenderId; set => SetField(ref _selectedSenderId, value); }
    public int DailyCap { get => _dailyCap; set => SetField(ref _dailyCap, value); }
    public int DelayMin { get => _delayMin; set => SetField(ref _delayMin, value); }
    public int DelayMax { get => _delayMax; set => SetField(ref _delayMax, value); }
    public int SelectedListId { get => _selectedListId; set => SetField(ref _selectedListId, value); }

    private async void LoadTemplatesAndSendersAsync(SenderAccountService senderService, TemplateService templateService)
    {
        var templates = await templateService.GetAllAsync();
        var senders = await senderService.ListAsync();
        var lists = await _listService.GetAllListsAsync();
        Templates.Clear(); foreach (var t in templates) Templates.Add(t);
        Senders.Clear(); foreach (var s in senders) Senders.Add(s);
        Lists.Clear(); foreach (var l in lists) Lists.Add(l);
        if (Templates.Count > 0) SelectedTemplateId = Templates[0].Id;
        if (Senders.Count > 0) SelectedSenderId = Senders[0].Id;
    }

    public async void LoadAsync()
    {
        var list = await _campaignService.GetAllAsync();
        Campaigns.Clear();
        foreach (var c in list) Campaigns.Add(c);
    }

    private async void CreateAsync()
    {
        if (string.IsNullOrWhiteSpace(NewName)) { MessageBox.Show("Enter campaign name."); return; }
        try
        {
            await _campaignService.CreateAsync(new CreateCampaignCommand(
                NewName, SelectedTemplateId, SelectedSenderId, DailyCap, DelayMin, DelayMax, null));
            NewName = "";
            LoadAsync();
            MessageBox.Show("Campaign created. Add recipients and Start.");
        }
        catch (Exception ex) { MessageBox.Show(ex.Message); }
    }

    private void AddSenderCap()
    {
        if (Senders.Count == 0) { MessageBox.Show("Add at least one sender account first."); return; }
        var pick = Senders.FirstOrDefault(s => !SenderCaps.Any(c => c.SenderAccountId == s.Id)) ?? Senders[0];
        SenderCaps.Add(new SenderCapRow { SenderAccountId = pick.Id, MaxEmails = 100, DisplayEmail = pick.Email });
    }

    private void RemoveSenderCap(object? parameter)
    {
        if (parameter is SenderCapRow row)
            SenderCaps.Remove(row);
    }

    private async void StartAsync()
    {
        if (SelectedCampaign == null) { MessageBox.Show("Select a campaign."); return; }
        if (SelectedListId == 0) { MessageBox.Show("Select a contact list for recipients."); return; }
        var listIds = new List<int> { SelectedListId };
        IReadOnlyList<(int SenderAccountId, int MaxEmails)>? senderCaps = null;
        if (SenderCaps.Count > 0)
        {
            senderCaps = SenderCaps.Select(r => (r.SenderAccountId, r.MaxEmails)).ToList();
            if (senderCaps.Any(x => x.MaxEmails <= 0)) { MessageBox.Show("Each sender must have Max emails > 0."); return; }
        }
        try
        {
            await _campaignService.StartCampaignAsync(SelectedCampaign.Id, listIds, senderCaps);
            LoadAsync();
            MessageBox.Show("Campaign started. Sends will be processed by the Worker.");
        }
        catch (Exception ex) { MessageBox.Show(ex.Message); }
    }

    private async void PauseAsync()
    {
        if (SelectedCampaign == null) return;
        try { await _campaignService.PauseCampaignAsync(SelectedCampaign.Id); LoadAsync(); }
        catch (Exception ex) { MessageBox.Show(ex.Message); }
    }

    private async void StopAsync()
    {
        if (SelectedCampaign == null) return;
        try { await _campaignService.StopCampaignAsync(SelectedCampaign.Id); LoadAsync(); }
        catch (Exception ex) { MessageBox.Show(ex.Message); }
    }
}
