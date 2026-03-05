namespace MailerApp.Application.Contacts;

public record CsvImportResult(int Imported, int Skipped, IReadOnlyList<string> Errors);
