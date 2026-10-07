# PostToolUse (Edit|Write on *.cs): formats whitespace in just the edited file. Never blocks.
$ErrorActionPreference = 'Stop'
try {
    $payload = [Console]::In.ReadToEnd() | ConvertFrom-Json
    $filePath = $payload.tool_input.file_path
    if (-not $filePath -or -not $filePath.EndsWith('.cs') -or -not (Test-Path -LiteralPath $filePath)) { exit 0 }

    $projectDir = $env:CLAUDE_PROJECT_DIR ?? $payload.cwd
    $relative = [IO.Path]::GetRelativePath($projectDir, $filePath)

    # --folder skips loading MSBuild, which keeps this around a second instead of ten.
    dotnet format whitespace $projectDir --folder --include $relative 2>&1 | Out-Null
    exit 0
}
catch {
    [Console]::Error.WriteLine("format-cs hook: skipped after unexpected error: $_")
    exit 0
}
