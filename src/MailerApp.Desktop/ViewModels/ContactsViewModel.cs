using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using MailerApp.Application.Contacts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;

namespace MailerApp.Desktop.ViewModels;

public class ContactsViewModel : ViewModelBase
{
    private readonly ContactImportService _importService;
    private readonly ContactListService _listService;
    private ContactListDto? _selectedList;
    private string? _newListName;
    private string? _newListDescription;

    public ContactsViewModel(ContactImportService importService, ContactListService listService)
    {
        _importService = importService;
        _listService = listService;
        Lists = new ObservableCollection<ContactListDto>();
        Contacts = new ObservableCollection<ContactDto>();
        CreateListCommand = new RelayCommand(_ => CreateListAsync());
        ImportCsvCommand = new RelayCommand(_ => ImportCsvAsync());
        RefreshListsCommand = new RelayCommand(_ => LoadListsAsync());
        RefreshContactsCommand = new RelayCommand(_ => LoadContactsAsync());
    }

    public ObservableCollection<ContactListDto> Lists { get; }
    public ObservableCollection<ContactDto> Contacts { get; }
    public ICommand CreateListCommand { get; }
    public ICommand ImportCsvCommand { get; }
    public ICommand RefreshListsCommand { get; }
    public ICommand RefreshContactsCommand { get; }

    public ContactListDto? SelectedList { get => _selectedList; set { if (SetField(ref _selectedList, value)) LoadContactsAsync(); } }
    public string? NewListName { get => _newListName; set => SetField(ref _newListName, value); }
    public string? NewListDescription { get => _newListDescription; set => SetField(ref _newListDescription, value); }

    public async void LoadListsAsync()
    {
        var list = await _listService.GetAllListsAsync();
        Lists.Clear();
        foreach (var l in list) Lists.Add(l);
    }

    private async void LoadContactsAsync()
    {
        Contacts.Clear();
        if (SelectedList == null) return;
        var list = await _listService.GetContactsByListAsync(SelectedList.Id);
        foreach (var c in list) Contacts.Add(c);
    }

    private async void CreateListAsync()
    {
        if (string.IsNullOrWhiteSpace(NewListName)) { MessageBox.Show("Enter list name."); return; }
        try
        {
            await _listService.CreateListAsync(NewListName.Trim(), NewListDescription?.Trim());
            NewListName = NewListDescription = null;
            LoadListsAsync();
            MessageBox.Show("List created.");
        }
        catch (Exception ex) { MessageBox.Show(GetFullErrorMessage(ex), "Error"); }
    }

    private async void ImportCsvAsync()
    {
        if (SelectedList == null) { MessageBox.Show("Select a list first."); return; }
        var dlg = new OpenFileDialog { Filter = "CSV|*.csv|All|*.*", Title = "Select CSV" };
        if (dlg.ShowDialog() != true) return;
        try
        {
            await using var stream = File.OpenRead(dlg.FileName);
            var result = await _importService.ImportCsvAsync(stream, SelectedList.Id);
            MessageBox.Show($"Imported: {result.Imported}, Skipped: {result.Skipped}. Errors: {result.Errors.Count}");
            LoadContactsAsync();
        }
        catch (Exception ex) { MessageBox.Show(GetFullErrorMessage(ex), "Error"); }
    }

    private static string GetFullErrorMessage(Exception ex)
    {
        if (ex is Microsoft.EntityFrameworkCore.DbUpdateException dbEx && dbEx.InnerException != null)
            return dbEx.InnerException.Message;
        var inner = ex.InnerException;
        return inner != null ? $"{ex.Message}\n\nDetails: {inner.Message}" : ex.Message;
    }
}
