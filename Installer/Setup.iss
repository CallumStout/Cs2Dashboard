#define MyAppId "{{}}"
#define MyAppName "CS2 Stats Dashboard"
#define MyAppVersion "1.0.0.0"
#define CompanyName "97 Solutions"
#define SetupName "CS2DashboardSetup"
#define AppIcon (SourcePath + "..\Artwork\")
#define PathToBinary (SourcePath + "..\..\Cs2Dashboard\bin\Release")
#define MyAppExeName "CS2 Dashboard.exe"

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

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "{#PathToBinary}\*"; DestDir: "{app}"; Flags: recursesubdirs createallsubdirs;

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Flags: nowait postinstall skipifsilent