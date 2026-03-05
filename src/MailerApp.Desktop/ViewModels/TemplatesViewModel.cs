using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using MailerApp.Application.Templates;

namespace MailerApp.Desktop.ViewModels;

public class TemplatesViewModel : ViewModelBase
{
    private readonly TemplateService _templateService;
    private TemplateDto? _selectedTemplate;
    private string? _editName;
    private string? _editSubject;
    private string? _editBodyHtml;
    private string? _previewSubject;
    private string? _previewBody;

    public TemplatesViewModel(TemplateService templateService)
    {
        _templateService = templateService;
        Templates = new ObservableCollection<TemplateDto>();
        Variables = new ObservableCollection<string>();
        LoadCommand = new RelayCommand(_ => LoadAsync());
        SaveCommand = new RelayCommand(_ => SaveAsync());
        NewCommand = new RelayCommand(_ => NewTemplate());
        DeleteCommand = new RelayCommand(_ => DeleteAsync());
        InsertVariableCommand = new RelayCommand(InsertVariable);
        PreviewCommand = new RelayCommand(_ => UpdatePreview());
    }

    public ObservableCollection<TemplateDto> Templates { get; }
    public ObservableCollection<string> Variables { get; }
    public ICommand LoadCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand NewCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand InsertVariableCommand { get; }
    public ICommand PreviewCommand { get; }

    public TemplateDto? SelectedTemplate { get => _selectedTemplate; set { if (SetField(ref _selectedTemplate, value)) LoadEditFields(); } }
    public string? EditName { get => _editName; set => SetField(ref _editName, value); }
    public string? EditSubject { get => _editSubject; set { if (SetField(ref _editSubject, value)) UpdateVariables(); } }
    public string? EditBodyHtml { get => _editBodyHtml; set { if (SetField(ref _editBodyHtml, value)) UpdateVariables(); } }
    public string? PreviewSubject { get => _previewSubject; set => SetField(ref _previewSubject, value); }
    public string? PreviewBody { get => _previewBody; set => SetField(ref _previewBody, value); }

    public async void LoadAsync()
    {
        var list = await _templateService.GetAllAsync();
        Templates.Clear();
        foreach (var t in list) Templates.Add(t);
    }

    private void LoadEditFields()
    {
        if (SelectedTemplate == null) { EditName = EditSubject = EditBodyHtml = null; return; }
        EditName = SelectedTemplate.Name;
        EditSubject = SelectedTemplate.Subject;
        EditBodyHtml = SelectedTemplate.BodyHtml ?? "";
        UpdateVariables();
    }

    private void UpdateVariables()
    {
        var vars = TemplateService.ExtractVariables(EditSubject, EditBodyHtml, null);
        Variables.Clear();
        foreach (var v in vars) Variables.Add(v);
    }

    private async void SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(EditName) || string.IsNullOrWhiteSpace(EditSubject)) { MessageBox.Show("Name and Subject required."); return; }
        try
        {
            if (SelectedTemplate != null)
                await _templateService.UpdateAsync(SelectedTemplate.Id, EditName, EditSubject, EditBodyHtml, null);
            else
                await _templateService.CreateAsync(new CreateTemplateCommand(EditName, EditSubject, EditBodyHtml, null));
            LoadAsync();
            MessageBox.Show("Saved.");
        }
        catch (Exception ex) { MessageBox.Show(ex.Message); }
    }

    private void NewTemplate()
    {
        SelectedTemplate = null;
        EditName = EditSubject = EditBodyHtml = "";
    }

    private async void DeleteAsync()
    {
        if (SelectedTemplate == null) return;
        MessageBox.Show("Delete not implemented in MVP - remove manually from DB if needed.");
    }

    private void InsertVariable(object? parameter)
    {
        if (parameter is not string v) return;
        var token = "{{" + v + "}}";
        EditBodyHtml = (EditBodyHtml ?? "") + token;
    }

    private void UpdatePreview()
    {
        PreviewSubject = ReplaceVariables(EditSubject ?? "", new Dictionary<string, string> { ["firstName"] = "John", ["company"] = "Acme", ["lastName"] = "Doe" });
        PreviewBody = ReplaceVariables(EditBodyHtml ?? "", new Dictionary<string, string> { ["firstName"] = "John", ["company"] = "Acme", ["lastName"] = "Doe" });
    }

    private static string ReplaceVariables(string text, IReadOnlyDictionary<string, string> values)
    {
        foreach (var (key, val) in values)
            text = text.Replace("{{" + key + "}}", val, StringComparison.OrdinalIgnoreCase);
        return text;
    }
}
