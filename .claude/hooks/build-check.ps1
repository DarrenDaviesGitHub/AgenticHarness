# Stop: if C#/project files changed since the last green build, runs a quiet build.
# On failure exits 2 so Claude fixes the errors before finishing. Tests are left to /verify.
$ErrorActionPreference = 'Stop'
try {
    $payload = [Console]::In.ReadToEnd() | ConvertFrom-Json
    if ($payload.stop_hook_active) { exit 0 }

    Set-Location ($env:CLAUDE_PROJECT_DIR ?? $payload.cwd)
    if (-not (git rev-parse --is-inside-work-tree 2>$null)) { exit 0 }

    # Fingerprint = HEAD + every changed build input with its timestamp and size.
    $patterns = '*.cs', '*.csproj', '*.props', '*.targets', '*.slnx', 'global.json', '.editorconfig'
    $changed = @(git status --porcelain=v1 --untracked-files=all -- $patterns 2>$null) | ForEach-Object {
        $path = $_.Substring(3).Trim('"')
        $item = Get-Item -LiteralPath $path -ErrorAction SilentlyContinue
        "$path|$($item.LastWriteTimeUtc.Ticks)|$($item.Length)"
    }
    $head = git rev-parse HEAD 2>$null
    $fingerprint = [Convert]::ToHexString(
        [Security.Cryptography.SHA256]::HashData([Text.Encoding]::UTF8.GetBytes("$head`n$($changed -join "`n")")))

    $stateFile = git rev-parse --git-path claude-build-check
    if ((Test-Path $stateFile) -and (Get-Content $stateFile -Raw).Trim() -eq $fingerprint) { exit 0 }

    $output = dotnet build -warnaserror -nologo -v q -clp:NoSummary 2>&1
    if ($LASTEXITCODE -eq 0) {
        Set-Content -Path $stateFile -Value $fingerprint
        exit 0
    }

    $errors = @($output | Where-Object { $_ -match '\b(error|warning)\b' } | ForEach-Object { ($_ -replace '\s*\[[^\]]+\.csproj\]$', '').Trim() } | Select-Object -Unique -First 30)
    if ($errors.Count -eq 0) { $errors = @($output | Select-Object -Last 30) }
    [Console]::Error.WriteLine("dotnet build -warnaserror failed. Fix these before finishing:`n$($errors -join "`n")")
    exit 2
}
catch {
    [Console]::Error.WriteLine("build-check hook: skipped after unexpected error: $_")
    exit 0
}
