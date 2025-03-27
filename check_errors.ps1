$output = dotnet build MicroSaaS.Infrastructure 2>&1
$errors = $output | Where-Object { $_ -match "error" }
if ($errors) {
    Write-Host "Errors found:"
    $errors | ForEach-Object { Write-Host $_ }
} else {
    Write-Host "No errors found"
} 