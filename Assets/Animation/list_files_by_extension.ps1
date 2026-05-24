while ($true) {
    Clear-Host

    Write-Host "=========================================="
    Write-Host "  FILE LIST BY EXTENSION"
    Write-Host "=========================================="
    Write-Host ("Folder: " + (Get-Location).Path)
    Write-Host ""

    $files = Get-ChildItem -LiteralPath . -File | Sort-Object Name

    if ($files.Count -eq 0) {
        Write-Host "No files found in this folder."
        Read-Host "Press Enter to close"
        break
    }

    $groups = $files | Group-Object Extension | Sort-Object Name

    Write-Host "[0] EXIT"
    Write-Host "[1] ALL FILES"

    for ($i = 0; $i -lt $groups.Count; $i++) {
        $extName = $groups[$i].Name
        if ([string]::IsNullOrWhiteSpace($extName)) {
            $extName = "[no extension]"
        }
        Write-Host ("[" + ($i + 2) + "] " + $extName + "  (" + $groups[$i].Count + ")")
    }

    Write-Host ""
    $choice = Read-Host "Enter number"

    if ($choice -notmatch '^\d+$') {
        Write-Host ""
        Write-Host "Invalid input."
        Read-Host "Press Enter to continue"
        continue
    }

    $choiceNum = [int]$choice

    if ($choiceNum -eq 0) {
        break
    }

    Clear-Host
    Write-Host "=========================================="

    if ($choiceNum -eq 1) {
        Write-Host "[ALL FILES]"
        Write-Host "------------------------------------------"

        foreach ($f in $files) {
            Write-Host $f.Name
        }

        Write-Host "------------------------------------------"
        Write-Host ("Total files: " + $files.Count)
    }
    elseif ($choiceNum -ge 2 -and $choiceNum -le ($groups.Count + 1)) {
        $selectedGroup = $groups[$choiceNum - 2]
        $selectedExt = $selectedGroup.Name

        if ([string]::IsNullOrWhiteSpace($selectedExt)) {
            Write-Host "[FILES WITH NO EXTENSION]"
        }
        else {
            Write-Host ("[FILES: " + $selectedExt + "]")
        }

        Write-Host "------------------------------------------"

        $selectedFiles = $selectedGroup.Group | Sort-Object Name
        foreach ($f in $selectedFiles) {
            Write-Host $f.Name
        }

        Write-Host "------------------------------------------"
        Write-Host ("Total files: " + $selectedFiles.Count)
    }
    else {
        Write-Host "Invalid number."
    }

    Write-Host "=========================================="
    Read-Host "Press Enter to return to menu"
}