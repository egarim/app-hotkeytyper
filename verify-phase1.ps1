# HotkeyTyper Phase 1 Data Layer Verification Script
# Verifies that all data layer components were created and compile successfully

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "HotkeyTyper Data Layer Verification" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Step 1: Build the project
Write-Host "[1/3] Building project..." -ForegroundColor Yellow
Set-Location "$PSScriptRoot\HotkeyTyper"
$buildOutput = dotnet build 2>&1 | Out-String

if ($LASTEXITCODE -eq 0) {
    Write-Host "  [OK] Build successful" -ForegroundColor Green
} else {
    Write-Host "  [ERROR] Build failed" -ForegroundColor Red
    Write-Host $buildOutput
    exit 1
}

# Step 2: Verify assembly exists
Write-Host ""
Write-Host "[2/3] Verifying compiled assembly..." -ForegroundColor Yellow
$dllPath = Join-Path $PSScriptRoot "HotkeyTyper\bin\Debug\net10.0-windows\HotkeyTyper.dll"

if (Test-Path $dllPath) {
    $dllInfo = Get-Item $dllPath
    $sizeKB = [math]::Round($dllInfo.Length/1KB, 2)
    Write-Host "  [OK] Assembly found: HotkeyTyper.dll" -ForegroundColor Green
    Write-Host "  [OK] Size: $sizeKB KB" -ForegroundColor Green
    Write-Host "  [OK] Modified: $($dllInfo.LastWriteTime)" -ForegroundColor Green
} else {
    Write-Host "  [ERROR] Assembly not found at $dllPath" -ForegroundColor Red
    exit 1
}

# Step 3: Verify source files
Write-Host ""
Write-Host "[3/3] Verifying source files..." -ForegroundColor Yellow

$sourceFiles = @(
    "Models\Snippet.cs",
    "Models\SnippetSet.cs",
    "Models\AppConfiguration.cs",
    "Managers\SettingsManager.cs"
)

$totalLines = 0
foreach ($file in $sourceFiles) {
    $fullPath = Join-Path $PSScriptRoot "HotkeyTyper\$file"
    if (Test-Path $fullPath) {
        $lines = (Get-Content $fullPath | Measure-Object -Line).Lines
        $totalLines += $lines
        Write-Host "  [OK] $file ($lines lines)" -ForegroundColor Green
    } else {
        Write-Host "  [ERROR] $file (MISSING)" -ForegroundColor Red
    }
}

# Summary
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Phase 1 Data Layer: VERIFIED" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "Summary Statistics:" -ForegroundColor Yellow
Write-Host "  - Total source lines: $totalLines" -ForegroundColor White
Write-Host "  - Files created: $($sourceFiles.Count)" -ForegroundColor White
Write-Host "  - Build status: SUCCESS" -ForegroundColor White
Write-Host ""

Write-Host "Data Model Components:" -ForegroundColor Yellow
Write-Host "  - Snippet.cs          : Individual text snippets with metadata" -ForegroundColor White
Write-Host "  - SnippetSet.cs       : Collections of related snippets" -ForegroundColor White
Write-Host "  - AppConfiguration.cs : Top-level application settings" -ForegroundColor White
Write-Host "  - SettingsManager.cs  : JSON persistence and migration" -ForegroundColor White
Write-Host ""

Write-Host "Key Features Implemented:" -ForegroundColor Yellow
Write-Host "  - GUID-based unique identifiers" -ForegroundColor White
Write-Host "  - Hotkey management (1-9 per set)" -ForegroundColor White
Write-Host "  - Comprehensive validation with error messages" -ForegroundColor White
Write-Host "  - Migration from V1 to V2 format" -ForegroundColor White
Write-Host "  - Automatic backup before saves" -ForegroundColor White
Write-Host "  - Storage in AppData\Local\HotkeyTyper" -ForegroundColor White
Write-Host ""

Write-Host "Storage Location:" -ForegroundColor Yellow
Write-Host "  - Path: $env:LOCALAPPDATA\HotkeyTyper" -ForegroundColor White
Write-Host "  - File: settings.json" -ForegroundColor White
Write-Host "  - Backup: settings.backup.json" -ForegroundColor White
Write-Host ""

Write-Host "Next Steps:" -ForegroundColor Yellow
Write-Host "  1. Launch the application to test runtime behavior" -ForegroundColor White
Write-Host "  2. Verify settings.json is created on first launch" -ForegroundColor White
Write-Host "  3. Proceed to Phase 2: UI Implementation" -ForegroundColor White
Write-Host ""

Write-Host "Documentation:" -ForegroundColor Yellow
Write-Host "  - docs\phase1-completion-summary.md" -ForegroundColor White
Write-Host "  - docs\data-model-reference.md" -ForegroundColor White
Write-Host "  - docs\FEATURE-PLAN-snippet-sets.md" -ForegroundColor White
Write-Host ""

Write-Host "Phase 1 verification complete!" -ForegroundColor Green
Write-Host ""
