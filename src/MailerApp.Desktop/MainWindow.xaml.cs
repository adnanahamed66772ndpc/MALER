using System.Windows;
using System.Windows.Controls;
using MailerApp.Desktop.Views;
using MailerApp.Desktop.ViewModels;
using MailerApp.Application.Campaigns;
using MailerApp.Application.Contacts;
using MailerApp.Application.Suppression;
using MailerApp.Application.SenderAccounts;
using MailerApp.Application.Templates;
using Microsoft.Extensions.DependencyInjection;

namespace MailerApp.Desktop;

public partial class MainWindow : Window
{
    private readonly IServiceProvider _serviceProvider;

    public MainWindow(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        InitializeComponent();
        NavList.SelectedIndex = 0;
    }

    private void NavList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (NavList.SelectedItem is not ListBoxItem item || item.Tag is not string tag) return;
        switch (tag)
        {
            case "Accounts":
                var accountsService = _serviceProvider.GetRequiredService<SenderAccountService>();
                var accountsVm = new AccountsViewModel(accountsService, _serviceProvider);
                accountsVm.LoadCommand.Execute(null);
                ContentArea.Content = new AccountsView { DataContext = accountsVm };
                break;
            case "Contacts":
                var importService = _serviceProvider.GetRequiredService<ContactImportService>();
                var listService = _serviceProvider.GetRequiredService<ContactListService>();
                var contactsVm = new ContactsViewModel(importService, listService);
                contactsVm.LoadListsAsync();
                ContentArea.Content = new ContactsView { DataContext = contactsVm };
                break;
            case "Templates":
                var templateService = _serviceProvider.GetRequiredService<TemplateService>();
                var templatesVm = new TemplatesViewModel(templateService);
                templatesVm.LoadCommand.Execute(null);
                ContentArea.Content = new TemplatesView { DataContext = templatesVm };
                break;
            case "Campaigns":
                var campaignService = _serviceProvider.GetRequiredService<CampaignService>();
                var contactListService = _serviceProvider.GetRequiredService<ContactListService>();
                var senderService = _serviceProvider.GetRequiredService<SenderAccountService>();
                var campTemplates = _serviceProvider.GetRequiredService<TemplateService>();
                var campaignsVm = new CampaignsViewModel(campaignService, contactListService, senderService, campTemplates);
                campaignsVm.LoadCommand.Execute(null);
                ContentArea.Content = new CampaignsView { DataContext = campaignsVm };
                break;
            case "Dashboard":
                var dashCampaignService = _serviceProvider.GetRequiredService<CampaignService>();
                var exportService = _serviceProvider.GetRequiredService<CampaignExportService>();
                var dashboardVm = new DashboardViewModel(dashCampaignService, exportService);
                dashboardVm.LoadCommand.Execute(null);
                ContentArea.Content = new DashboardView { DataContext = dashboardVm };
                break;
            case "Settings":
                var suppressionService = _serviceProvider.GetRequiredService<SuppressionListService>();
                var updateService = _serviceProvider.GetRequiredService<MailerApp.Desktop.Services.UpdateService>();
                var settingsVm = new SettingsViewModel(suppressionService, updateService);
                settingsVm.LoadSuppressionCommand.Execute(null);
                ContentArea.Content = new SettingsView { DataContext = settingsVm };
                break;
            default:
                ContentArea.Content = null;
                break;
        }
    }
}