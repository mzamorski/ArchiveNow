msiexec /i ArchiveNow.Setup.msi /l*v log.txt

srm install ArchiveNow.Shell.dll -codebase
srm uninstall ArchiveNow.Shell.dll && taskkill /F /IM explorer.exe && explorer.exe

--[Examples]------------------------------------------------------------------------------------------------------------

--paths E:\PROJEKTY\W£ASNE\ArchiveNow\tmp --profile-file "E:\PROJEKTY\W£ASNE\ArchiveNow\trunk\env\Profiles\Test.profile"