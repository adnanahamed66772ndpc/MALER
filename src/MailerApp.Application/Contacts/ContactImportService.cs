using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using MailerApp.Domain.Entities;
using MailerApp.Domain.Interfaces;

namespace MailerApp.Application.Contacts;

public class ContactImportService
{
    private readonly IContactRepository _contacts;
    private readonly IContactListRepository _lists;

    public ContactImportService(IContactRepository contacts, IContactListRepository lists)
    {
        _contacts = contacts;
        _lists = lists;
    }

    /// <summary>Import CSV stream into the given list. Columns: name, company, email (required). Dedupe by email (keep first).</summary>
    public async Task<CsvImportResult> ImportCsvAsync(Stream csvStream, int listId, CancellationToken cancellationToken = default)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true, MissingFieldFound = null };
        using var reader = new StreamReader(csvStream);
        using var csv = new CsvReader(reader, config);
        var rows = new List<CsvContactRow>();
        await foreach (var record in csv.GetRecordsAsync<CsvContactRow>(cancellationToken))
            rows.Add(record);
        var existingEmails = await _contacts.GetExistingEmailsAsync(rows.Where(r => !string.IsNullOrWhiteSpace(r.Email)).Select(r => r.Email!.Trim()), cancellationToken);
        var existingSet = existingEmails.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var errors = new List<string>();
        var toAdd = new List<Contact>();
        var seenInFile = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var skippedCount = 0;
        foreach (var row in rows)
        {
            var email = row.Email?.Trim();
            if (string.IsNullOrEmpty(email)) { errors.Add($"Row with name '{row.Name}' has no email"); continue; }
            if (!IsValidEmail(email)) { errors.Add($"Invalid email: {email}"); continue; }
            if (seenInFile.Contains(email)) { errors.Add($"Duplicate in file: {email}"); continue; }
            seenInFile.Add(email);
            if (existingSet.Contains(email)) { skippedCount++; continue; }
            var firstName = row.FirstName?.Trim() ?? row.Name?.Trim();
            var lastName = row.LastName?.Trim();
            toAdd.Add(new Contact
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                Company = row.Company?.Trim()
            });
            existingSet.Add(email);
        }
        if (toAdd.Count > 0)
        {
            await _contacts.AddRangeAsync(toAdd, cancellationToken);
            var contactIds = toAdd.Select(c => c.Id).ToList();
            await _lists.AddMembersAsync(listId, contactIds, cancellationToken);
        }
        return new CsvImportResult(toAdd.Count, skippedCount, errors);
    }

    private static bool IsValidEmail(string email)
    {
        try { var _ = new System.Net.Mail.MailAddress(email); return true; }
        catch { return false; }
    }

    private class CsvContactRow
    {
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Company { get; set; }
    }
}
