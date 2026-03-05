# Mailer App

Windows email-sending desktop application (C# .NET, WPF) with Gmail OAuth/SMTP, contact lists, templates, campaigns, and a background send worker.

## Requirements

- .NET 10 SDK (or .NET 8+ with target adjustment)
- Windows (for DPAPI token encryption; fallback encryption on other OS)

## Windows যাতে অ্যাপ ব্লক না করে (SmartScreen)

অ্যাপে একটি **app manifest** যোগ করা আছে যাতে Windows অ্যাপটাকে সাধারণ ডেস্কটপ অ্যাপ হিসেবে চিনে। **পুরোপুরি ব্লক/সতর্কতা এড়াতে** EXE এবং ইনস্টলার **code sign** করুন (যেকোনো Code Signing Certificate দিয়ে)। বিস্তারিত: [docs/Windows-SmartScreen-And-Signing.md](docs/Windows-SmartScreen-And-Signing.md).

## Solution structure

- **MailerApp.Desktop** – WPF UI (Accounts, Contacts, Templates, Campaigns, Dashboard, Settings)
- **MailerApp.Application** – Use cases, DTOs, FluentValidation, send engine
- **MailerApp.Domain** – Entities and interfaces
- **MailerApp.Infrastructure** – EF Core SQLite, Gmail API/SMTP, token encryption
- **MailerApp.Worker** – Background service that processes the send queue
- **MailerApp.Tests** – Unit/integration tests

## Git দিয়ে EXE তৈরি (GitHub Actions)

কোড push করলেই GitHub অটো বিল্ড করে EXE তৈরি করবে। **Actions** ট্যাব → লেটেস্ট রান → **Artifacts** থেকে `MailerApp-Desktop-EXE` (ZIP) ডাউনলোড করুন। বিস্তারিত: [docs/Git-Build-EXE.md](docs/Git-Build-EXE.md).

## Run

1. **Desktop (UI)**  
   From repo root:
   ```bash
   dotnet run --project src/MailerApp.Desktop
   ```
   Database and logs are under `%LocalAppData%\MailerApp`.

2. **Worker (send queue)**  
   Run in a separate terminal so sends are processed:
   ```bash
   dotnet run --project src/MailerApp.Worker
   ```

3. **Gmail**  
   - Copy `appsettings.json` (or create from sample) and set `GmailOAuth:ClientId` and `GmailOAuth:ClientSecret` from Google Cloud Console (OAuth 2.0 credentials, redirect URI e.g. `http://localhost:8080/`).
   - For SMTP: add a Gmail account with an App Password.

## Auto-update (upgrade without uninstall)

- **Settings → Updates**: Shows current version, “Check for updates”, and “Download and install”. Configure `Update:FeedUrl` in `appsettings.json` to point to a JSON feed with `version`, `downloadUrl`, and optional `releaseNotes`. When the user installs the new version, it **replaces** the old one in place (no uninstall needed).
- **Installer**: Use the Inno Setup script in `installer/MailerApp.iss`; it uses a fixed AppId so running a newer installer upgrades in place. See `docs/Auto-Update-Setup.md` for the feed format and build steps.

## Installer (Week 5)

For a Windows installer:

1. Publish the Desktop app:
   ```bash
   dotnet publish src/MailerApp.Desktop -c Release -r win-x64 --self-contained -o src/MailerApp.Desktop/bin/Release/net10.0-windows/publish
   ```
2. Compile `installer/MailerApp.iss` with Inno Setup (see `docs/Auto-Update-Setup.md`).
3. Publish the Worker the same way if you want it as a separate executable.

## MVP features

- Gmail OAuth and Gmail SMTP sender accounts (credentials stored encrypted)
- Contact lists and CSV import with dedupe
- Templates with merge fields (e.g. `{{firstName}}`, `{{company}}`) and preview
- Campaigns: create, add recipients from lists, start/pause/stop; send queue with delay and retry
- Dashboard: campaign summary and CSV export of recipients
- Suppression list and compliance reminder (SPF/DKIM/DMARC, unsubscribe)

## License

MIT (or your choice).
