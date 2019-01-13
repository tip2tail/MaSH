; Script generated by the Inno Script Studio Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "MaSH"
#define MyAppVersion "1.1.0"
#define MyAppPublisher "tip2tail Ltd"
#define MyAppURL "https://www.tip2tail.scot/"
#define MyAppExeName "MaSH.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{90628772-B7D6-46DA-9FA3-74DF73373F51}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={userappdata}\tip2tail\{#MyAppName}
DefaultGroupName={#MyAppName}
LicenseFile=D:\Repos\MaSH\LICENSE.txt
OutputDir=.\Installer
OutputBaseFilename=MaSH_Setup
Compression=lzma
SolidCompression=yes
PrivilegesRequired=lowest

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "D:\Repos\MaSH\MaSH\bin\Release\MaSH.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\Repos\MaSH\MaSH\bin\Release\MaSH.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\Repos\MaSH\MaSH\bin\Release\MaSH.pdb"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\Repos\MaSH\MaSH\bin\Release\Newtonsoft.Json.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\Repos\MaSH\MaSH\bin\Release\Newtonsoft.Json.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\Repos\MaSH\LICENSE.txt"; DestDir: "{app}"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent
