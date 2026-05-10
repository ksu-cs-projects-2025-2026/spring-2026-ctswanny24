param(
    [string]$SqlPassword = "RecipEZ_dev_2026!"
)

$ErrorActionPreference = "Stop"

function Require-Command {
    param([string]$Name)

    if (-not (Get-Command $Name -ErrorAction SilentlyContinue)) {
        throw "Missing required command: $Name"
    }
}

function Wait-For-SqlServer {
    param(
        [string]$HostName = "localhost",
        [int]$Port = 1433,
        [int]$TimeoutSeconds = 90
    )

    $deadline = (Get-Date).AddSeconds($TimeoutSeconds)

    while ((Get-Date) -lt $deadline) {
        $client = New-Object System.Net.Sockets.TcpClient
        try {
            $connect = $client.BeginConnect($HostName, $Port, $null, $null)
            if ($connect.AsyncWaitHandle.WaitOne(1000, $false)) {
                $client.EndConnect($connect)
                return
            }
        }
        catch {
            Start-Sleep -Seconds 2
        }
        finally {
            $client.Close()
        }
    }

    throw "SQL Server did not become available on $HostName`:$Port within $TimeoutSeconds seconds."
}

$repoRoot = Split-Path -Parent $PSScriptRoot
Set-Location $repoRoot

Require-Command "docker"
Require-Command "dotnet"
Require-Command "node"
Require-Command "npm"

Write-Host "Starting SQL Server container..."
docker compose up -d sqlserver
Wait-For-SqlServer

Write-Host "Restoring .NET packages..."
dotnet restore .\Recip-EZ.sln

Write-Host "Installing client packages..."
Push-Location .\recip-ez.client
npm install
Pop-Location

$env:ASPNETCORE_ENVIRONMENT = "Development"
$env:ConnectionStrings__DefaultConnection = "Server=localhost,1433;Database=Recip-EZ_Database;User Id=sa;Password=$SqlPassword;TrustServerCertificate=True;MultipleActiveResultSets=true;"

Write-Host "Starting Vite client in a separate PowerShell window..."
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$repoRoot\recip-ez.client'; npm run dev"

Write-Host "Starting ASP.NET API. The database will be recreated and seeded by Program.cs..."
dotnet run --project .\Recip-EZ.Server\Recip-EZ.Server.csproj

