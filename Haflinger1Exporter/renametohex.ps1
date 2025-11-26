function ConvertTo-HexEncodedFilename {
    param(
        [string]$FileName
    )
    
    # Define characters that should be hex-encoded
    # Excludes: alphanumeric, dash, dot
    $charsToEncode = @('_', '(', ')', '[', ']', '{', '}', ' ', '&', '%', '$', '#', '@', '!', '+', '=', ',', ';', "'", '`')
    
    $result = ""
    foreach ($char in $FileName.ToCharArray()) {
        if ($charsToEncode -contains $char) {
            # Convert to hex format with flipped byte order: _XXYY_ where XX and YY are swapped
            $hexValue = [String]::Format("{0:X4}", [int][char]$char)
            # Flip the byte order: take last 2 chars first, then first 2 chars
            $flippedHex = $hexValue.Substring(2, 2) + $hexValue.Substring(0, 2)
            $result += "_$flippedHex`_"
        }
        else {
            # Keep the character as-is (alphanumeric, dash, dot, etc.)
            $result += $char
        }
    }
    
    return $result
}

# Get all files recursively in the current directory and subdirectories
# Process files from deepest to shallowest to avoid path issues
Get-ChildItem -Path . -File -Recurse | Sort-Object -Property FullName -Descending | ForEach-Object {
    $file = $_
    
    # Get just the filename without extension and the extension separately
    $nameWithoutExt = [System.IO.Path]::GetFileNameWithoutExtension($file.Name)
    $extension = $file.Extension
    
    # Encode the filename (without extension)
    $encodedName = ConvertTo-HexEncodedFilename -FileName $nameWithoutExt
    
    # Reconstruct the full filename with extension
    $newName = $encodedName + $extension
    
    # Only rename if the name actually changed
    if ($newName -ne $file.Name) {
        # Get the full path for the new name
        $newPath = Join-Path -Path $file.DirectoryName -ChildPath $newName
        
        # Check if a file with the new name already exists
        if (Test-Path -Path $newPath) {
            Write-Warning "Cannot rename '$($file.FullName)' - file '$newPath' already exists"
        }
        else {
            try {
                # Rename the file
                Rename-Item -Path $file.FullName -NewName $newName -ErrorAction Stop
                Write-Host "Renamed: $($file.Name) -> $newName" -ForegroundColor Green
            }
            catch {
                Write-Error "Failed to rename '$($file.FullName)': $_"
            }
        }
    }
}

Write-Host "`nRenaming complete!" -ForegroundColor Cyan