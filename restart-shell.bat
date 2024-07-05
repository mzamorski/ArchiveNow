@echo off


set ProjectBinFolder=D:\__PROJEKTY\WùASNE\ArchiveNow\trunk\bin\Debug
set ProjectSrcFolder=%cd%
rem set ProjectRootFolder=%ProjectSrcFolder%

set AppFolder=D:\Program Files (x86)\ArchiveNow
set RoamingAppFolder=C:\Users\Marcin\AppData\Roaming\ArchiveNow\Profiles
set SrmPath="%AppFolder%\srm.exe"

cd "%AppFolder%"

%SrmPath% uninstall ArchiveNow.Shell.dll && taskkill /F /IM explorer.exe && explorer.exe

copy "%ProjectBinFolder%\*.dll" "%AppFolder%"
copy "%ProjectBinFolder%\*.exe" "%AppFolder%"

%SrmPath% install ArchiveNow.Shell.dll -codebase

cd "%ProjectSrcFolder%"

explorer.exe %cd%
rem explorer.exe "%AppFolder%"
explorer.exe "%RoamingAppFolder%"
explorer.exe "%ProjectSrcFolder%"

