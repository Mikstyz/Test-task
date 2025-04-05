$OutputEncoding = [Console]::OutputEncoding = [System.Text.Encoding]::UTF8

$csFiles = Get-ChildItem -Recurse -Filter *.cs
$totalLines = 0
$totalChars = 0

foreach ($file in $csFiles) {
    $lines = Get-Content $file.FullName
    foreach ($line in $lines) {
        $trimmed = $line.Trim() -replace "\s", ""
        if ($trimmed.Length -gt 0) {
            $totalLines++
            $totalChars += $trimmed.Length
        }
    }
}

Write-Host "=============================="
Write-Host "Количество непустых строк: $totalLines"
Write-Host "Количество символов (без пробелов, табов и пустот): $totalChars"
Write-Host "=============================="
Read-Host "Нажми Enter, чтобы закрыть эту грязную консольку"
