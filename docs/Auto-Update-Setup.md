# Auto-Update Setup

The Mailer app can update itself **without uninstalling**: when a new version is available, you run the new installer (or use **Settings → Check for updates → Download and install**). The new installer **replaces** the old files in the same folder, so your data (e.g. `%LocalAppData%\MailerApp`) is kept.

## 1. Update feed (for "Check for updates")

The app checks a **JSON feed URL** for a newer version. Configure it in `appsettings.json`:

```json
"Update": {
  "FeedUrl": "https://your-server.com/mailer/update-feed.json"
}
```

**Feed format** (one JSON file at that URL):

```json
{
  "version": "1.2.0",
  "downloadUrl": "https://your-server.com/releases/MailerApp-Setup-1.2.0.exe",
  "releaseNotes": "Bug fixes and improvements."
}
```

- `version` – latest version (e.g. `1.2.0`). The app compares this to its current version.
- `downloadUrl` – direct link to the **installer** (e.g. the Inno Setup EXE).
- `releaseNotes` – optional text shown in Settings when an update is available.

When the user clicks **Download and install**, the app downloads the installer from `downloadUrl`, runs it, and closes. The installer (Inno Setup) installs over the existing folder, so no uninstall is needed.

## 2. Building the installer (Inno Setup) for upgrade-in-place

1. **Publish** the Desktop app:
   ```bash
   dotnet publish src/MailerApp.Desktop -c Release -r win-x64 --self-contained -o src/MailerApp.Desktop/bin/Release/net10.0-windows/publish
   ```

2. **Set the version** in:
   - `src/MailerApp.Desktop/MailerApp.Desktop.csproj`: `<Version>1.2.0</Version>` (and `AssemblyVersion`, `FileVersion`, `InformationalVersion`).
   - `installer/MailerApp.iss`: `#define MyAppVersion "1.2.0"` and `#define PublishDir "..\src\MailerApp.Desktop\bin\Release\net10.0-windows\publish"` (match your publish path).

3. **Compile** the installer with [Inno Setup](https://jrsoftware.org/isinfo.php):
   - Open `installer/MailerApp.iss`.
   - Build → Compile.
   - The EXE is in `installer/output/`, e.g. `MailerApp-Setup-1.2.0.exe`.

4. **Important for upgrades**: The script uses a **fixed AppId**. When the user runs a **new** version’s installer (e.g. 1.2.0) and the app is already installed (e.g. 1.0.0), Inno Setup will **upgrade in place** (same folder, files replaced). The user does **not** need to uninstall first.

## 3. Hosting the feed and installer

- Put `update-feed.json` on a web server or GitHub Releases (e.g. use the API or a static file).
- Put each `MailerApp-Setup-X.Y.Z.exe` on the same server or GitHub Releases and set `downloadUrl` in the feed to that file’s URL.
- When you release 1.2.0: update the feed’s `version` and `downloadUrl`, then users who click **Check for updates** will see the new version and can **Download and install** to upgrade without uninstalling.

## 4. Optional: silent upgrade from the app

When the app runs the installer after **Download and install**, it does **not** pass `/VERYSILENT`. To make the upgrade fully silent (no installer UI), you can change `UpdateService.RunInstallerAndExit` to pass:

```csharp
Arguments = "/VERYSILENT"
```

Then the installer will replace files in the background and the user can start the app again from the Start menu or desktop shortcut.
