#define MyAppId "{{4BEACCCC-4709-4D2C-A9BB-3AEAA5D78527}}"
#define MyAppName "CS2 Stats Dashboard"
#define MyAppVersion "1.0.0.0"
#define CompanyName "97 Solutions"
#define SetupName "CS2DashboardSetup"
#define AppIcon (SourcePath + "Artwork\")
#define PathToBinary (SourcePath + "..\cs2dashboard\bin\Release\net8.0\win-x64\publish")
#define MyAppExeName "Cs2Dashboard.exe"

[Setup]
AppId={#MyAppId}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
DefaultDirName={commonpf64}\{#CompanyName}\{#MyAppName}
DefaultGroupName={#CompanyName}\{#MyAppName}
OutputDir=..\Release\97Solutions
OutputBaseFilename={#SetupName}
Compression=lzma
SolidCompression=yes
UsePreviousAppDir=yes
UsePreviousGroup=yes
PrivilegesRequired=admin
SetupIconFile="{#AppIcon}App.ico"
UninstallDisplayIcon={app}\{#MyAppExeName}
WizardStyle=modern
WizardImageFile="{#AppIcon}InnoLarge.bmp"
WizardSmallImageFile="{#AppIcon}InnoSmall.bmp"
CloseApplications=yes
CloseApplicationsFilter={#MyAppExeName}
RestartApplications=no

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "{#PathToBinary}\\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Flags: nowait postinstall skipifsilent
