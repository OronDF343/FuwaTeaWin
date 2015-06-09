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
Source: "..\bin\Release\FTWPlayer.exe"; DestDir: "{app}"; Components: main
Source: "..\bin\Release\FuwaTea.Wpf.dll"; DestDir: "{app}"; Components: main
Source: "..\bin\Release\FuwaTea.Presentation.dll"; DestDir: "{app}"; Components: main
Source: "..\bin\Release\FuwaTea.Logic.dll"; DestDir: "{app}"; Components: main
Source: "..\bin\Release\FuwaTea.Data.dll"; DestDir: "{app}"; Components: main
Source: "..\bin\Release\FuwaTea.Common.dll"; DestDir: "{app}"; Components: main
Source: "..\bin\Release\LayerFramework.dll"; DestDir: "{app}"; Components: main
Source: "..\bin\Release\FuwaTea.Lib.dll"; DestDir: "{app}"; Components: main
Source: "..\bin\Release\GalaSoft.MvvmLight.dll"; DestDir: "{app}"; Components: main
Source: "..\bin\Release\GalaSoft.MvvmLight.Extras.dll"; DestDir: "{app}"; Components: main
Source: "..\bin\Release\GalaSoft.MvvmLight.Platform.dll"; DestDir: "{app}"; Components: main
Source: "..\bin\Release\Microsoft.Practices.ServiceLocation.dll"; DestDir: "{app}"; Components: main
Source: "..\bin\Release\System.Windows.Interactivity.dll"; DestDir: "{app}"; Components: main
Source: "..\bin\Release\WPFLocalizeExtension.dll"; DestDir: "{app}"; Components: main
Source: "..\bin\Release\XAMLMarkupExtensions.dll"; DestDir: "{app}"; Components: main
Source: "..\bin\Release\TagLib.Portable.dll"; DestDir: "{app}"; Components: main
Source: "..\bin\Release\NAudio.dll"; DestDir: "{app}"; Components: main
Source: "..\bin\Release\NAudio.Flac.dll"; DestDir: "{app}"; Components: main
Source: "..\bin\Release\NAudio.Vorbis.dll"; DestDir: "{app}"; Components: main
Source: "..\bin\Release\NVorbis.dll"; DestDir: "{app}"; Components: main
Source: "..\bin\Release\he\FTWPlayer.resources.dll"; DestDir: "{app}\he"; Components: localization\hebrew

[Icons]
Name: "{commondesktop}\FTW Player"; Filename: "{app}\FTWPlayer.exe"; WorkingDir: "{app}"; Tasks: desktopicon
Name: "{group}\FTW Player"; Filename: "{app}\FTWPlayer.exe"; WorkingDir: "{app}"
Name: "{group}\{cm:UninstallIcon,FTW Player}"; Filename: "{uninstallexe}"; Flags: excludefromshowinnewinstall

[InstallDelete]
Type: filesandordirs; Name: "{localappdata}\OronDF343\FuwaTeaWin\*"; Tasks: reset

[UninstallDelete]
Type: filesandordirs; Name: "{localappdata}\OronDF343\FuwaTeaWin"
Type: dirifempty; Name: "{localappdata}\OronDF343"

[Languages]
Name: "en"; MessagesFile: "compiler:Default.isl,msg_en.isl"; LicenseFile: "LICENSE.txt"
Name: "he"; MessagesFile: "compiler:Languages\Hebrew.isl,msg_he.isl"; LicenseFile: "LICENSE_he.txt"

[Registry]
Root: HKLM; Subkey: "Software\OronDF343"; Flags: uninsdeletekeyifempty
Root: HKLM; Subkey: "Software\OronDF343\FuwaTeaWin"; Flags: uninsdeletekey; ValueType: string; ValueName: "InstallLocation"; ValueData: "{app}"
Root: HKCR; Subkey: "FuwaTeaWin.AudioFileGeneric"; Flags: uninsdeletekey; ValueType: string; ValueData: "{cm:FileExtGeneric}"
Root: HKCR; Subkey: "FuwaTeaWin.AudioFileGeneric\DefaultIcon"; ValueType: string; ValueData: """{app}\FTWPlayer.exe"",0"
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
 