param (
    [Parameter(Mandatory=$true)]
    [string]$PlayHomePath,

    [Parameter(Mandatory=$true)]
    [ValidateSet("Debug", "Release")]
    [string]$BuildConfiguration,

    [string]$DotNetPath = "C:\Program Files\dotnet"
)

# paths
$applicationProcess = "PlayHome64bit"
$exePath = Join-Path -Path $PlayHomePath -ChildPath "$applicationProcess.exe"
$outputDll = "..\bin\BepInEx\plugins\Ash.dll"
$targetDir = Join-Path -Path $PlayHomePath -ChildPath "BepInEx\plugins\Ash"

# env
$env:DOTNET_ROOT = $DotNetPath
$env:PATH = "$env:DOTNET_ROOT;$env:PATH"

# dll copying
$retryCount = 0
$maxRetries = 5
$copySuccess = $false

# expection handling
$exitCode = 0


try {
    Write-Host "Running Build command (Debug)" -ForegroundColor Cyan
    dotnet build --configuration $BuildConfiguration
    if ($LASTEXITCODE -ne 0) {
        $exitCode = $LASTEXITCODE
        throw "Build failed."
    }


    $process = Get-Process -Name $applicationProcess -ErrorAction SilentlyContinue
    if ($process) {
        Write-Host "Terminating existing process.."
        Stop-Process -Name $applicationProcess -Force

        Write-Host "Waiting for process to exit.."
        $process | Wait-Process
    }


    if (-not (Test-Path -Path $targetDir)) {
        throw [System.IO.DirectoryNotFoundException]::new("Target directory '$targetDir' not found.")
    }

    do {
        try {
            Copy-Item -Path $outputDll -Destination $targetDir -Force -ErrorAction Stop
            $copySuccess = $true
        } catch {
            $retryCount++
            if ($retryCount -lt $maxRetries) {
                Write-Host "File is locked. " -ForegroundColor Yellow -NoNewline
                Write-Host "Retrying in 1 second.. ($retryCount/$maxRetries)"
                Start-Sleep -Seconds 1
            } else {
                throw [System.IO.IOException]::new("Failed to copy after $maxRetries attempts. The file is locked.")
            }
        }
    } while (-not $copySuccess)

    Write-Host "Build process exited"


    Write-Host "Starting application.."
    if (Test-Path $exePath) {
        Start-Process -FilePath $exePath
    } else {
        throw [System.IO.IOException]::new("Could not find the executable at '$exePath'.")
    }
} catch [System.IO.IOException] {
    $exitCode = "0x{0:X8}" -f $_.Exception.HResult
    Write-Host "$($_.Exception.Message)" -ForegroundColor Red
} catch {
    $exitCode = 1
    Write-Host "$($_.Exception.Message)" -ForegroundColor Red
} finally {
    if ($exitCode -eq 0) {
        Write-Host "Build script exited with code $exitCode" -ForegroundColor Green
    } else {
        Write-Host "Build script exited with code $exitCode. See https://learn.microsoft.com/en-us/openspecs/windows_protocols/ms-erref/705fb797-2175-4a90-b5a3-3918024b10b8 for more information." -ForegroundColor Red
    }

    exit $exitCode
}
