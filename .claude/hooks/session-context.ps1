# SessionStart: prints repo state into Claude's context. Never blocks.
$ErrorActionPreference = 'Stop'
try {
    $null = [Console]::In.ReadToEnd()
    Set-Location ($env:CLAUDE_PROJECT_DIR ?? (Get-Location))

    $lines = [System.Collections.Generic.List[string]]::new()
    if (git rev-parse --is-inside-work-tree 2>$null) {
        $branch = git branch --show-current 2>$null
        $dirty = @(git status --porcelain 2>$null).Count
        $lines.Add("Branch: $($branch ? $branch : '(detached)') | Uncommitted files: $dirty")
        $commits = @(git log --oneline -5 2>$null)
        $lines.Add('Last commits:')
        if ($commits.Count -eq 0) { $lines.Add('  (none yet)') } else { $commits | ForEach-Object { $lines.Add("  $_") } }
    }
    else {
        $lines.Add('Not a git repository yet (run: git init).')
    }

    $lines.Add(".NET SDK: $(dotnet --version 2>$null)")
    $lines.Add('Reminder: run /verify before committing.')
    $lines -join "`n" | Write-Output
    exit 0
}
catch {
    [Console]::Error.WriteLine("session-context hook: skipped after unexpected error: $_")
    exit 0
}
