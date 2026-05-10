param(
    [string]$RepoUrl = "https://github.com/ksu-cs-projects-2025-2026/spring-2026-ctswanny24.git",
    [string]$FolderName = "spring-2026-ctswanny24"
)

$ErrorActionPreference = "Stop"

if (-not (Get-Command git -ErrorAction SilentlyContinue)) {
    throw "Missing required command: git"
}

if (-not (Test-Path $FolderName)) {
    git clone $RepoUrl $FolderName
}

Set-Location $FolderName
.\scripts\run-dev.ps1

