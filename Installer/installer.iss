[Setup]
AppId=FuwaTeaWin
AppName=FTW Player
AppPublisher=OronDF343
AppPublisherURL=https://orondf343.wordpress.com/
AppSupportURL=https://github.com/OronDF343/FuwaTeaWin/issues
AppUpdatesURL=https://github.com/OronDF343/FuwaTeaWin/releases
AppVersion=0.0.0.1
VersionInfoVersion=0.0.0.1
AppCopyright=Copyright (C) 2015 OronDF343

OutputBaseFilename=ftwplayer_setup
SolidCompression=yes

MinVersion=6.0
DefaultDirName={reg:HKLM\Software\OronDF343\FuwaTeaWin,Path|{pf}\FTW Player}
DisableDirPage=auto
DefaultGroupName=FTW Player
DisableProgramGroupPage=yes
AppMutex=FuwaTeaWin|Installed
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
Name: "{localappdata}\OronDF343\FuwaTeaWin"; Components: main

[Files]
Source: "..\bin\Release\FTWPlayer.exe"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "..\bin\Release\FuwaTea.Wpf.dll"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "..\bin\Release\FuwaTea.Presentation.dll"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "..\bin\Release\FuwaTea.Logic.dll"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "..\bin\Release\FuwaTea.Data.dll"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "..\bin\Release\FuwaTea.Common.dll"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "..\bin\Release\LayerFramework.dll"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "..\bin\Release\FuwaTea.Lib.dll"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "..\bin\Release\GalaSoft.MvvmLight.dll"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "..\bin\Release\GalaSoft.MvvmLight.Extras.dll"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "..\bin\Release\GalaSoft.MvvmLight.Platform.dll"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "..\bin\Release\Microsoft.Practices.ServiceLocation.dll"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "..\bin\Release\System.Windows.Interactivity.dll"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "..\bin\Release\WPFLocalizeExtension.dll"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "..\bin\Release\XAMLMarkupExtensions.dll"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "..\bin\Release\TagLib.Portable.dll"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "..\bin\Release\NAudio.dll"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "..\bin\Release\NAudio.Flac.dll"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "..\bin\Release\NAudio.Vorbis.dll"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "..\bin\Release\NVorbis.dll"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "..\bin\Release\he\FTWPlayer.resources.dll"; DestDir: "{app}\he"; Components: localization\hebrew; Flags: ignoreversion

[Icons]
Name: "{commondesktop}\FTW Player"; Filename: "{app}\FTWPlayer.exe"; WorkingDir: "{app}"; Tasks: desktopicon
Name: "{group}\FTW Player"; Filename: "{app}\FTWPlayer.exe"; WorkingDir: "{app}"
Name: "{group}\{cm:UninstallIcon,FTW Player}"; Filename: "{uninstallexe}"; Flags: excludefromshowinnewinstall

[InstallDelete]
Type: filesandordirs; Name: "{localappdata}\OronDF343\FuwaTeaWin\*"; Tasks: reset

[UninstallDelete]
Type: filesandordirs; Name: "{localappdata}\OronDF343\FuwaTeaWin"; Check: CheckDelUserData
Type: dirifempty; Name: "{localappdata}\OronDF343"; Check: CheckDelUserData

[Languages]
Name: "en"; MessagesFile: "compiler:Default.isl,msg_en.isl"; LicenseFile: "LICENSE.txt"
Name: "he"; MessagesFile: "compiler:Languages\Hebrew.isl,msg_he.isl"; LicenseFile: "LICENSE_he.txt"

[Registry]
Root: HKLM; Subkey: "Software\OronDF343"; Flags: uninsdeletekeyifempty
Root: HKLM; Subkey: "Software\OronDF343\FuwaTeaWin"; Flags: uninsdeletekey; ValueType: string; ValueName: "InstallLocation"; ValueData: "{app}"
Root: HKCR; Subkey: "FuwaTeaWin.AudioFileGeneric"; Flags: uninsdeletekey; ValueType: string; ValueData: "{cm:FileExtGeneric}"
Root: HKCR; Subkey: "FuwaTeaWin.AudioFileGeneric\DefaultIcon"; ValueType: string; ValueData: """{app}\FTWPlayer.exe"",0"
Root: HKCR; Subkey: "FuwaTeaWin.AudioFileGeneric\shell"; ValueType: string; ValueData: "Play"
Root: HKCR; Subkey: "FuwaTeaWin.AudioFileGeneric\shell\Play"; ValueType: string; ValueData: "{cm:PlayWith,FTW Player}"
Root: HKCR; Subkey: "FuwaTeaWin.AudioFileGeneric\shell\Play\command"; ValueType: string; ValueData: """{app}\FTWPlayer.exe"" ""%1"""
Root: HKCR; Subkey: "FuwaTeaWin.AudioFileGeneric\shell\AddToPlaylist"; ValueType: string; ValueData: "{cm:AddToPlaylist,FTW Player}"
Root: HKCR; Subkey: "FuwaTeaWin.AudioFileGeneric\shell\AddToPlaylist\command"; ValueType: string; ValueData: """{app}\FTWPlayer.exe"" ""%1"" --add"
Root: HKCR; Subkey: "Directory\shell\PlayFTW"; Flags: uninsdeletekey; ValueType: string; ValueData: "{cm:PlayWith,FTW Player}"
Root: HKCR; Subkey: "Directory\shell\PlayFTW\command"; ValueType: string; ValueData: """{app}\FTWPlayer.exe"" ""%1"""
Root: HKCR; Subkey: "Directory\shell\AddToPlaylistFTW"; Flags: uninsdeletekey; ValueType: string; ValueData: "{cm:AddToPlaylist,FTW Player}"
Root: HKCR; Subkey: "Directory\shell\AddToPlaylistFTW\command"; ValueType: string; ValueData: """{app}\FTWPlayer.exe"" ""%1"" --add"
Root: HKLM; Subkey: "Software\Clients\Media\FuwaTeaWin"; Flags: uninsdeletekey
Root: HKLM; Subkey: "Software\Clients\Media\FuwaTeaWin\Capabilities"; ValueName: "ApplicationName"; ValueType: string; ValueData: "FTW Player"
Root: HKLM; Subkey: "Software\Clients\Media\FuwaTeaWin\Capabilities"; ValueName: "ApplicationDescription"; ValueType: string; ValueData: "{cm:AppDescription}"
Root: HKLM; Subkey: "Software\Clients\Media\FuwaTeaWin\Capabilities\FileAssociations"
Root: HKLM; Subkey: "Software\RegisteredApplications"; Flags: uninsdeletevalue; ValueName: "FuwaTeaWin"; ValueType: string; ValueData: "Software\Clients\Media\FuwaTeaWin\Capabilities"

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
        if MsgBox('Would you like to remove the user preferences as well?', mbConfirmation, MB_YESNO) = IDYES then
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
//    'v4.6\RC'        .NET Framework 4.5.2
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
    if not IsDotNetDetected('v4.5.1', 0) then begin
        MsgBox('This application requires Microsoft .NET Framework 4.5.1.'#13#13
               'Please use Windows Update to install this version,'#13
               'and then re-run this setup program.', mbInformation, MB_OK);
        result := false;
    end else
        result := true;
end;
