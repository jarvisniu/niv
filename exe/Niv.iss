; Niv_en.iss
; Configuration file for Inno Setup which is an installer generator.

#define appName "Niv"
#define authorName "Jarvis Niu"
#define officialWebsite "http://jarvisniu.com/niv"

#define version "0.4.7"

#define regAppName "Niv"
#define exeFileName "Niv.exe"

[Setup]
AppName={#version}
AppVersion={#appName}
AppPublisher={#authorName}
AppSupportURL={#officialWebsite}
DefaultDirName={pf}\{#appName}
DefaultGroupName={#appName}
UninstallDisplayIcon={app}\{#exeFileName}
UninstallDisplayName={#appName}
Compression=lzma2
SolidCompression=yes
OutputDir=".\"
OutputBaseFilename="{#appName}_v{#version}_setup"
ChangesAssociations = yes

[Files]
Source: "Niv.exe"; DestDir: "{app}"

[Icons]
Name: "{group}\Niv"; Filename: "{app}\{#exeFileName}"
Name: "{userdesktop}\{#appName}"; Filename: "{app}\{#exeFileName}";

[Registry]
Root: HKCR; Subkey: ".jpg";                             ValueData: "{#regAppName}";         Flags: uninsdeletevalue;  ValueType: string;  ValueName: ""
Root: HKCR; Subkey: "{#regAppName}";                     ValueData: "Program {#regAppName}";   Flags: uninsdeletekey;  ValueType: string;  ValueName: ""
Root: HKCR; Subkey: "{#regAppName}\DefaultIcon";         ValueData: "{app}\{#exeFileName},0";           ValueType: string;  ValueName: ""
Root: HKCR; Subkey: "{#regAppName}\shell\open\command";  ValueData: """{app}\{#exeFileName}"" ""%1""";  ValueType: string;  ValueName: ""
