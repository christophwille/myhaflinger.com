Get-ChildItem -Path . -File -Recurse | ForEach-Object {
    # Check if the filename contains spaces
    if ($_.Name -match '\s') {
        # Create new name by replacing spaces with dashes
        $newName = $_.Name -replace '\s', '-'
        
        # Get the full path for the new name
        $newPath = Join-Path -Path $_.DirectoryName -ChildPath $newName
        
        # Check if a file with the new name already exists
        if (Test-Path -Path $newPath) {
            Write-Warning "Cannot rename '$($_.FullName)' - file '$newPath' already exists"
        }
        else {
            # Rename the file
            Rename-Item -Path $_.FullName -NewName $newName -Verbose
        }
    }
}