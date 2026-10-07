# PreToolUse (Edit|Write) for the qa-engineer subagent only: it may change files under tests/ and nothing else.
$ErrorActionPreference = 'Stop'
try {
    $payload = [Console]::In.ReadToEnd() | ConvertFrom-Json
    $filePath = $payload.tool_input.file_path
    if (-not $filePath) { exit 0 }

    $projectDir = $env:CLAUDE_PROJECT_DIR ?? $payload.cwd
    $relative = [IO.Path]::GetRelativePath($projectDir, $filePath) -replace '\\', '/'
    if ($relative -like 'tests/*') { exit 0 }

    [Console]::Error.WriteLine("Blocked: qa-engineer may only edit files under tests/ (tried $relative). Report production-code issues instead of fixing them.")
    exit 2
}
catch {
    [Console]::Error.WriteLine("qa-tests-only hook: allowing after unexpected error: $_")
    exit 0
}
