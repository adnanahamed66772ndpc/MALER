; Inno Setup script for Mailer App - upgrade-in-place (no uninstall needed)
; Build: publish app first, then compile this script with the output path below.

#define MyAppName "Mailer App"
#define MyAppVersion "1.0.0"
; IMPORTANT: Use a single fixed AppId so running a NEW installer is treated as an UPGRADE (same app, replace files).
#define MyAppId "A1B2C3D4-E5F6-7890-ABCD-EF1234567890"
#define MyAppPublisher "Mailer"
#define MyAppURL "https://github.com/your-repo/mailer"
#define PublishDir "..\src\MailerApp.Desktop\bin\Release\net10.0-windows\publish"

[Setup]
AppId={{#MyAppId}}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
; Output: single EXE installer
OutputDir=output
OutputBaseFilename=MailerApp-Setup-{#MyAppVersion}
SetupIconFile=
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
; Upgrade behavior: same AppId + same DefaultDirName = installer will REPLACE files in place (no uninstall required)
PrivilegesRequired=currentUser
CloseApplications=yes
RestartApplications=no

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"

[Files]
; Publish output: all files from publish folder
Source: "{#PublishDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\MailerApp.Desktop.exe"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\MailerApp.Desktop.exe"; Tasks: desktopicon

[Run]
; Optional: do not auto-launch after install so user can run when ready
; Filename: "{app}\MailerApp.Desktop.exe"; Description: "Launch {#MyAppName}"; Flags: nowait postinstall skipifsilent

[Code]
// Optional: detect existing install and show "Upgrade" instead of "Install"
function InitializeSetup(): Boolean;
begin
  Result := True;
end;
