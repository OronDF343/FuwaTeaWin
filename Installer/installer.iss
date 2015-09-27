
#define ShortName 'FuwaTeaWin'
#define LongName 'FTW Player'
#define Company 'OronDF343'
#define DotNetVersion '4.5.1'
#define BuildPath '..\bin\Release'
 
#ifndef VersionNumber
#define VersionNumber '0.0.0.0'
#endif

[Setup]
AppId={#ShortName}
AppName={#LongName}
AppPublisher={#Company}
AppPublisherURL=https://orondf343.wordpress.com/
AppSupportURL=https://github.com/{#Company}/{#ShortName}/issues
AppUpdatesURL=https://github.com/{#Company}/{#ShortName}/releases
AppVersion={#VersionNumber}
VersionInfoVersion={#VersionNumber}
AppCopyright=Copyright (C) 2015 {#Company}

OutputBaseFilename=ftwplayer_setup
SolidCompression=yes
SetupMutex=FTWSetup

MinVersion=6.0
DefaultDirName={reg:HKLM\Software\{#Company}\{#ShortName},Path|{pf}\{#LongName}}
DisableDirPage=auto
DefaultGroupName={#LongName}
DisableProgramGroupPage=yes
AppMutex={#ShortName}|Installed
AppModifyPath="{app}\FTWPlayer.exe" --modify

ChangesAssociations=yes
ArchitecturesInstallIn64BitMode=x64

[Components]
Name: "main"; Description: "{cm:MainFiles}"; Types: full compact custom; Flags: fixed
Name: "localization"; Description: "{cm:GUILanguages}"; Types: full custom
Name: "localization\hebrew"; Description: "{cm:Hebrew}"; Types: full custom

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:Icons}"; Components: main
Name: "associate"; Description: "{cm:UpdateFileAssociations}"; GroupDescription: "{cm:Tasks}"
Name: "reset"; Description: "{cm:ResetConfig}"; GroupDescription: "{cm:Tasks}"; Flags: unchecked

[Dirs]
Name: "{userappdata}\{#Company}\{#ShortName}"; Components: main
Name: "{app}\extensions"; Components: main
Name: "{app}\skins"; Components: main

[Files]
Source: "{#BuildPath}\FTWPlayer.exe"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "{#BuildPath}\FuwaTea.Wpf.dll"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "{#BuildPath}\FuwaTea.Presentation.dll"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "{#BuildPath}\FuwaTea.Logic.dll"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "{#BuildPath}\FuwaTea.Data.dll"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "{#BuildPath}\FuwaTea.Common.dll"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "{#BuildPath}\LayerFramework.dll"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "{#BuildPath}\FuwaTea.Lib.dll"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "{#BuildPath}\GalaSoft.MvvmLight.dll"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "{#BuildPath}\GalaSoft.MvvmLight.Extras.dll"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "{#BuildPath}\GalaSoft.MvvmLight.Platform.dll"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "{#BuildPath}\Microsoft.Practices.ServiceLocation.dll"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "{#BuildPath}\System.Windows.Interactivity.dll"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "{#BuildPath}\WPFLocalizeExtension.dll"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "{#BuildPath}\XAMLMarkupExtensions.dll"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "{#BuildPath}\Hardcodet.Wpf.TaskbarNotification.dll"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "{#BuildPath}\log4net.dll"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "{#BuildPath}\logconfig.xml"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "{#BuildPath}\TagLib.Portable.dll"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "{#BuildPath}\NAudio.dll"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "{#BuildPath}\NAudio.Vorbis.dll"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "{#BuildPath}\NVorbis.dll"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "{#BuildPath}\CSCore.dll"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "..\LICENSES_thirdparty.txt"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "{#BuildPath}\he\FTWPlayer.resources.dll"; DestDir: "{app}\he"; Components: localization\hebrew; Flags: ignoreversion

[Icons]
Name: "{commondesktop}\{#LongName}"; Filename: "{app}\FTWPlayer.exe"; WorkingDir: "{app}"; Tasks: desktopicon
Name: "{group}\{#LongName}"; Filename: "{app}\FTWPlayer.exe"; WorkingDir: "{app}"
Name: "{group}\{cm:UninstallIcon,{#LongName}}"; Filename: "{uninstallexe}"; Flags: excludefromshowinnewinstall

[InstallDelete]
Type: filesandordirs; Name: "{userappdata}\{#Company}\{#ShortName}\*"; Tasks: reset

[UninstallDelete]
Type: filesandordirs; Name: "{userappdata}\{#Company}\{#ShortName}"; Check: CheckDelUserData
Type: dirifempty; Name: "{userappdata}\{#Company}"; Check: CheckDelUserData
Type: files; Name: "{app}\ftw.log*"

[Languages]
Name: "en"; MessagesFile: "compiler:Default.isl,msg_en.isl"; LicenseFile: "..\LICENSE.txt"
Name: "he"; MessagesFile: "compiler:Languages\Hebrew.isl,msg_he.isl"; LicenseFile: "gplv3-hebrew.txt"

[Registry]
Root: HKLM; Subkey: "Software\{#Company}"; Flags: uninsdeletekeyifempty
Root: HKLM; Subkey: "Software\{#Company}\{#ShortName}"; Flags: uninsdeletekey; ValueType: string; ValueName: "InstallLocation"; ValueData: "{app}"
Root: HKCR; Subkey: "{#ShortName}.AudioFileGeneric"; Flags: uninsdeletekey; ValueType: string; ValueData: "{cm:FileExtGeneric}"
Root: HKCR; Subkey: "{#ShortName}.AudioFileGeneric\DefaultIcon"; ValueType: string; ValueData: """{app}\FTWPlayer.exe"",0"
Root: HKCR; Subkey: "{#ShortName}.AudioFileGeneric\shell"; ValueType: string; ValueData: "Play"
Root: HKCR; Subkey: "{#ShortName}.AudioFileGeneric\shell\Play"; ValueType: string; ValueData: "{cm:PlayWith,{#LongName}}"
Root: HKCR; Subkey: "{#ShortName}.AudioFileGeneric\shell\Play\command"; ValueType: string; ValueData: """{app}\FTWPlayer.exe"" ""%1"""
Root: HKCR; Subkey: "{#ShortName}.AudioFileGeneric\shell\AddToPlaylist"; ValueType: string; ValueData: "{cm:AddToPlaylist,{#LongName}}"
Root: HKCR; Subkey: "{#ShortName}.AudioFileGeneric\shell\AddToPlaylist\command"; ValueType: string; ValueData: """{app}\FTWPlayer.exe"" ""%1"" --add"
Root: HKCR; Subkey: "Directory\shell\PlayFTW"; Flags: uninsdeletekey; ValueType: string; ValueData: "{cm:PlayWith,{#LongName}}"
Root: HKCR; Subkey: "Directory\shell\PlayFTW\command"; ValueType: string; ValueData: """{app}\FTWPlayer.exe"" ""%1"""
Root: HKCR; Subkey: "Directory\shell\AddToPlaylistFTW"; Flags: uninsdeletekey; ValueType: string; ValueData: "{cm:AddToPlaylist,{#LongName}}"
Root: HKCR; Subkey: "Directory\shell\AddToPlaylistFTW\command"; ValueType: string; ValueData: """{app}\FTWPlayer.exe"" ""%1"" --add"
Root: HKLM; Subkey: "Software\Clients\Media\{#ShortName}"; Flags: uninsdeletekey
Root: HKLM; Subkey: "Software\Clients\Media\{#ShortName}\Capabilities"; ValueName: "ApplicationName"; ValueType: string; ValueData: "{#LongName}"
Root: HKLM; Subkey: "Software\Clients\Media\{#ShortName}\Capabilities"; ValueName: "ApplicationDescription"; ValueType: string; ValueData: "{cm:AppDescription}"
Root: HKLM; Subkey: "Software\Clients\Media\{#ShortName}\Capabilities\FileAssociations"
Root: HKLM; Subkey: "Software\RegisteredApplications"; Flags: uninsdeletevalue; ValueName: "FuwaTeaWin"; ValueType: string; ValueData: "Software\Clients\Media\{#ShortName}\Capabilities"

[Run]
Filename: "{app}\FTWPlayer.exe"; Parameters: "--setup-file-associations"; StatusMsg: "{cm:UpdatingFileAssociations}"; Tasks: associate
Filename: "{app}\FTWPlayer.exe"; Parameters: "--configure-file-associations"; Description: "{cm:ConfigureFileAssociations}"; Flags: postinstall

[UninstallRun]
Filename: "{app}\FTWPlayer.exe"; Parameters: "--clean-up-file-associations"; RunOnceId: "CUFA"
 
[Code]
var
    delUserData: boolean;

procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
begin
    if CurUninstallStep = usUninstall then
        if MsgBox(ExpandConstant('{cm:DelUserData}'), mbConfirmation, MB_YESNO) = IDYES then
            delUserData := true;
end;

function CheckDelUserData(): boolean;
begin
    result := delUserData;
end;

// Source: http://www.kynosarges.org/DotNetVersion.html
// Updated to .NET 4.5.1, 4.5.2, 4.6RC
function IsDotNetDetected(version: string; service: cardinal): boolean;
// Indicates whether the specified version and service pack of the .NET Framework is installed.
//
// version -- Specify one of these strings for the required .NET Framework version:
//    'v1.1.4322'     .NET Framework 1.1
//    'v2.0.50727'    .NET Framework 2.0
//    'v3.0'          .NET Framework 3.0
//    'v3.5'          .NET Framework 3.5
//    'v4\Client'     .NET Framework 4.0 Client Profile
//    'v4\Full'       .NET Framework 4.0 Full Installation
//    'v4.5'          .NET Framework 4.5
//    'v4.5.1'        .NET Framework 4.5.1
//    'v4.5.2'        .NET Framework 4.5.2
//    'v4.6\RC'        .NET Framework 4.6 RC
//
// service -- Specify any non-negative integer for the required service pack level:
//    0               No service packs required
//    1, 2, etc.      Service pack 1, 2, etc. required
var
    key, verCopy: string;
    install, release, serviceCount: cardinal;
    check45, success: boolean;
begin
    // .NET 4.5 installs as update to .NET 4.0 Full
    if Pos('v4.', version) = 1 then begin
		verCopy := version;
        version := 'v4\Full';
        check45 := true;
    end else
        check45 := false;

    // installation key group for all .NET versions
    key := 'SOFTWARE\Microsoft\NET Framework Setup\NDP\' + version;

    // .NET 3.0 uses value InstallSuccess in subkey Setup
    if Pos('v3.0', version) = 1 then begin
        success := RegQueryDWordValue(HKLM, key + '\Setup', 'InstallSuccess', install);
    end else begin
        success := RegQueryDWordValue(HKLM, key, 'Install', install);
    end;

    // .NET 4.0/4.5 uses value Servicing instead of SP
    if Pos('v4', version) = 1 then begin
        success := success and RegQueryDWordValue(HKLM, key, 'Servicing', serviceCount);
    end else begin
        success := success and RegQueryDWordValue(HKLM, key, 'SP', serviceCount);
    end;

    // .NET 4.5+ uses additional value Release
    if check45 then begin
        success := success and RegQueryDWordValue(HKLM, key, 'Release', release);
		if verCopy = 'v4.5.1' then
		    success := success and (release >= 378675)
		else if verCopy = 'v4.5.2' then
		    success := success and (release >= 379893)
		else if verCopy = 'v4.6\RC' then
		    success := success and (release >= 393273)
		else
            success := success and (release >= 378389);
    end;

    result := success and (install = 1) and (serviceCount >= service);
end;

function InitializeSetup(): Boolean;
begin
    if not IsDotNetDetected('v{#DotNetVersion}', 0) then begin
        MsgBox(ExpandConstant('{cm:NoDotNet, {#DotNetVersion}}'), mbInformation, MB_OK);
        result := false;
    end else
        result := true;
end;
