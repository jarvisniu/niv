; Niv_zh-CN.iss
; Configuration file for Inno Setup which is an installer generator.

[Setup]
AppName=小牛看图
AppVersion=0.3.2
AppPublisher=牛俊为
AppSupportURL=http://jarvisniu.com/niv
DefaultDirName={pf}\小牛看图
DefaultGroupName=小牛看图
UninstallDisplayIcon={app}\Niv.exe
UninstallDisplayName=小牛看图
Compression=lzma2
SolidCompression=yes
OutputDir=".\"
OutputBaseFilename="小牛看图_Setup_0.3.2"
;SetupIconFile=..\res\Niv.ico

[Files]
Source: "Niv.exe"; DestDir: "{app}"

[Icons]
Name: "{group}\Niv"; Filename: "{app}\Niv.exe"
