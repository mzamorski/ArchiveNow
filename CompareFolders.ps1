param(
    [Parameter(Mandatory=$true)]
    [string]$SourceDir,

    [Parameter(Mandatory=$true)]
    [string]$TargetDir
)

function Get-FileHashOrNA {
    param(
        [string]$FullPath
    )
    if (Test-Path $FullPath) {
        return (Get-FileHash $FullPath -Algorithm SHA256).Hash
    }
    else {
        return "N/A"
    }
}

function Compare-FoldersByHash {
    param(
        [string]$SourceDir,
        [string]$TargetDir
    )

    $files1 = Get-ChildItem -Recurse -File $SourceDir | 
        Select-Object @{Name='Path'; Expression = { $_.FullName.Substring($SourceDir.Length) } }, FullName

    $files2 = Get-ChildItem -Recurse -File $TargetDir | 
        Select-Object @{Name='Path'; Expression = { $_.FullName.Substring($TargetDir.Length) } }, FullName

    $hashTableSource = @{}
    foreach ($file in $files1) {
        $hashTableSource[$file.Path] = Get-FileHashOrNA $file.FullName
    }

    $hashTableTarget = @{}
    foreach ($file in $files2) {
        $hashTableTarget[$file.Path] = Get-FileHashOrNA $file.FullName
    }

    $allPaths = $hashTableSource.Keys + $hashTableTarget.Keys | Sort-Object -Unique

    $differencesFound = $false

    foreach ($path in $allPaths) {
        $hashSource = if ($hashTableSource.ContainsKey($path)) { $hashTableSource[$path] } else { "N/A" }
        $hashTarget = if ($hashTableTarget.ContainsKey($path)) { $hashTableTarget[$path] } else { "N/A" }

        if ($hashSource -ne $hashTarget) {
            $differencesFound = $true
            if ($hashSource -eq "N/A") {
                Write-Output "File missing in SourceDir: $path"
                Write-Output "  Target hash: $hashTarget"
            }
            elseif ($hashTarget -eq "N/A") {
                Write-Output "File missing in TargetDir: $path"
                Write-Output "  Source hash: $hashSource"
            }
            else {
                Write-Output "File differs: $path"
                Write-Output "  Source hash: $hashSource"
                Write-Output "  Target hash: $hashTarget"
            }
        }
    }

    if (-not $differencesFound) {
        Write-Output "Folders are identical in terms of file paths and content hashes."
    }
}

Compare-FoldersByHash -SourceDir $SourceDir -TargetDir $TargetDir
