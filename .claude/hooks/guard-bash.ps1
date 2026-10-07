# PreToolUse (Bash|PowerShell): backstop to the deny rules for destructive commands. Exit 2 = blocked.
$ErrorActionPreference = 'Stop'

# Recursive deletes are allowed only when every target is a bin/ or obj/ folder.
function Get-RecursiveDeleteReason([string] $part) {
    $tokens = @($part -split '\s+' | Where-Object { $_ })
    $verbIndex = [Array]::FindIndex($tokens, [Predicate[string]] { param($t) $t -in 'rm', 'Remove-Item', 'ri', 'del', 'rmdir', 'rd' })
    if ($verbIndex -lt 0) { return $null }

    $arguments = @($tokens | Select-Object -Skip ($verbIndex + 1))
    $flags = @($arguments | Where-Object { $_ -like '-*' })
    $targets = @($arguments | Where-Object { $_ -notlike '-*' } | ForEach-Object { $_.Trim('"', "'") })
    # Short Unix flags (-r, -rf, -Rf), --recursive, or any PowerShell abbreviation of -Recurse (-rec, -Recurse).
    $recursive = @($flags | Where-Object {
            ($_ -cmatch '^-[a-zA-Z]{1,3}$' -and $_ -cmatch '[rR]') -or $_ -eq '--recursive' -or
            ($_.Length -ge 4 -and '-Recurse'.StartsWith($_, [StringComparison]::OrdinalIgnoreCase))
        }).Count -gt 0
    if (-not $recursive) { return $null }

    $outsideBuildOutput = @($targets | Where-Object { $_ -notmatch '(^|[\\/])(bin|obj)[\\/]?$' })
    if ($targets.Count -gt 0 -and $outsideBuildOutput.Count -eq 0) { return $null }
    return 'Recursive deletes are only allowed on bin/ or obj/ folders.'
}

try {
    $payload = [Console]::In.ReadToEnd() | ConvertFrom-Json
    $command = [string]$payload.tool_input.command
    if (-not $command) { exit 0 }

    $reason = $null

    # Check each subcommand on its own so chaining cannot hide a dangerous one.
    foreach ($part in ($command -split '&&|\|\||;|\r?\n')) {
        $part = $part.Trim()
        $reason = switch -Regex ($part) {
            '\bgit\b.*\bpush\b.*(\s-f\b|\s--force|\s\+\S)' { 'Force-pushing rewrites shared history. Push normally or ask the user.'; break }
            '\bgit\b.*\breset\b.*--hard\b' { 'git reset --hard discards work irreversibly. Use git stash or ask the user.'; break }
            '\bdotnet\s+ef\s+database\s+drop\b' { 'Dropping the database destroys data. Ask the user to run it themselves.'; break }
            default { Get-RecursiveDeleteReason $part }
        }
        if ($reason) { break }
    }

    # A download piped or substituted into a shell spans the separators, so check the whole command.
    $download = '\b(curl|wget|iwr|irm|Invoke-WebRequest|Invoke-RestMethod)\b'
    $shell = '\b(sh|bash|zsh|pwsh|powershell|iex|Invoke-Expression)\b'
    if (-not $reason -and (
            $command -match "$download[^|]*\|\s*(sudo\s+)?$shell" -or
            $command -match "\b(iex|Invoke-Expression)\b[\s(]*$download" -or
            $command -match "\b(sh|bash|zsh)\b[^|]*(<\(|\`$\()\s*$download")) {
        $reason = 'Piping a download straight into a shell runs unreviewed code. Download, inspect, then run.'
    }

    if ($reason) {
        [Console]::Error.WriteLine("Blocked by guard-bash hook: $reason")
        exit 2
    }
    exit 0
}
catch {
    [Console]::Error.WriteLine("guard-bash hook: allowing after unexpected error: $_")
    exit 0
}
