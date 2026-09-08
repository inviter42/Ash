param (
    [Parameter(Mandatory=$true)]
    [string]$PlayHomePath,

    [Parameter(Mandatory=$true)]
    [ValidateSet("Debug", "Release")]
    [string]$BuildConfiguration,
    [string]$ApplicationProcess,
    [string]$ProjectName,
    [string]$OutputName,
    [string]$EsfName,

    [string]$DotNetPath = "C:\Program Files\dotnet"
)

# paths
$exePath = Join-Path -Path $PlayHomePath -ChildPath "$ApplicationProcess.exe"
$outputDll = "..\bin\BepInEx\plugins\$OutputName.dll"
$outputPDB = "..\bin\BepInEx\plugins\$OutputName.pdb"
$esfDll = "..\bin\BepInEx\plugins\$EsfName.dll"
$esfPDB = "..\bin\BepInEx\plugins\$EsfName.pdb"
$targetDir = Join-Path -Path $PlayHomePath -ChildPath "BepInEx\plugins\$ProjectName"
$projectPath = ".\$ProjectName\$ProjectName.csproj"

# env
$env:DOTNET_ROOT = $DotNetPath
$env:PATH = "$env:DOTNET_ROOT;$env:PATH"

# dll copying
$retryCount = 0
$maxRetries = 5
$copySuccess = $false

# expection handling
$exitCode = 0

function Test-FileLock {
    param ([string]$Path)
    if (-not (Test-Path $Path)) { return $false }
    try {
        # Try to open the file with exclusive access
        $stream = [System.IO.File]::Open($Path, 'Open', 'Read', 'None')
        $stream.Close()
        return $false # Not locked
    } catch {
        return $true  # Locked
    }
}

try {
    Write-Host "Running Build command ($BuildConfiguration)" -ForegroundColor Cyan
    dotnet clean $projectPath
    dotnet build $projectPath --configuration $BuildConfiguration
    if ($LASTEXITCODE -ne 0) {
        $exitCode = $LASTEXITCODE
        throw "Build failed."
    }


    $process = Get-Process -Name $ApplicationProcess -ErrorAction SilentlyContinue
    if ($process) {
        Write-Host "Terminating existing process.."
        Stop-Process -Name $ApplicationProcess -Force

        Write-Host "Waiting for process to exit.."
        $process | Wait-Process
    }


    if (-not (Test-Path -Path $targetDir)) {
        throw [System.IO.DirectoryNotFoundException]::new("Target directory '$targetDir' not found.")
    }

    do {
        # 1. Wait out the compiler if the PDB is still being written to disk
        if ($BuildConfiguration -eq "Debug" -and (Test-FileLock -Path $outputPDB)) {
            Write-Host "Compiler is still finalizing PDB file. Retrying..." -ForegroundColor Yellow
            Start-Sleep -Milliseconds 200
            $retryCount++
            continue
        }

        try {
            # 2. Perform the copy operations once the file is free
            Copy-Item -Path $outputDll -Destination $targetDir -Force -ErrorAction Stop
            Copy-Item -Path $esfDll -Destination $targetDir -Force -ErrorAction Stop
            if ($BuildConfiguration -eq "Debug") {
                Copy-Item -Path $outputPDB -Destination $targetDir -Force -ErrorAction Stop
                Copy-Item -Path $esfPDB -Destination $targetDir -Force -ErrorAction Stop
            }
            $copySuccess = $true
        } catch {
            $retryCount++
            if ($retryCount -lt $maxRetries) {
                Write-Host "File copy failed. Retrying in 1 second.. ($retryCount/$maxRetries)" -ForegroundColor Yellow
                Start-Sleep -Seconds 1
            } else {
                throw [System.IO.IOException]::new("Failed to copy after $maxRetries attempts.")
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
