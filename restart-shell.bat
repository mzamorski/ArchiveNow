@echo off


set ProjectBinFolder=E:\__PROJECTS\MY\ArchiveNow\ArchiveNow\bin\Debug
set ProjectSrcFolder=%cd%
rem set ProjectRootFolder=%ProjectSrcFolder%

set AppFolder=C:\Program Files (x86)\ArchiveNow
set AppFolderDriveLetter=%AppFolder:~0,2%
set RoamingAppFolder=C:\Users\marci\AppData\Roaming\ArchiveNow\Profiles
set SrmPath="%AppFolder%\srm.exe"


if exist "%AppFolder%" (
	%AppFolderDriveLetter%
    cd "%AppFolder%"
    echo Directory changed to: "%AppFolder%"
) else (
    echo The directory "%AppFolder%" does not exist.
	exit /b
)

echo The current directory is: "%CD%"

%SrmPath% uninstall ArchiveNow.Shell.dll && taskkill /F /IM explorer.exe && explorer.exe

 for /r "%AppFolder%" %%f in (*) do (
        if /i not "%%~nxf"=="srm.exe" if /i not "%%~nxf"=="ArchiveNow.conf" del /f /q "%%f"
    )

copy "%ProjectBinFolder%\*.dll" "%AppFolder%"
copy "%ProjectBinFolder%\*.exe" "%AppFolder%"

%SrmPath% install ArchiveNow.Shell.dll -codebase

cd "%ProjectSrcFolder%"

explorer.exe %cd%
rem explorer.exe "%AppFolder%"
explorer.exe "%RoamingAppFolder%"
explorer.exe "%ProjectSrcFolder%"

