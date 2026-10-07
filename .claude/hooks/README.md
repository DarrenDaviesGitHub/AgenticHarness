# Claude Code hooks

Hooks are the deterministic guard rails: `CLAUDE.md` and `.claude/rules/` guide Claude, hooks and permission rules enforce. They are configured in `.claude/settings.json` (and, for `qa-tests-only`, in the `qa-engineer` agent frontmatter).

**Requirement:** PowerShell 7+ (`pwsh`) on `PATH` on every OS (`winget install Microsoft.PowerShell`, `brew install powershell`, or your Linux package manager). Hooks run in exec form (`pwsh -NoProfile -File ${CLAUDE_PROJECT_DIR}/...`), so they do not depend on Git Bash or the default shell.

| Script | Event / matcher | What it does | Blocks? |
| --- | --- | --- | --- |
| `session-context.ps1` | `SessionStart` `startup\|resume\|compact` | Adds branch, uncommitted file count, last 5 commits, .NET SDK version and a `/verify` reminder to the context | Never |
| `protect-files.ps1` | `PreToolUse` `Edit\|Write` | Rejects edits to `.env*`, existing `Migrations/` files, `docs/openapi.json`, `bin/`, `obj/`, `.git/`, certificates and keys | Exit 2 with the reason and what to do instead |
| `guard-bash.ps1` | `PreToolUse` `Bash\|PowerShell` | Backstop to the deny rules: force-push, `reset --hard`, recursive deletes outside `bin/`/`obj/`, `dotnet ef database drop`, downloads piped into a shell. Checks every chained subcommand | Exit 2 with the reason |
| `format-cs.ps1` | `PostToolUse` `Edit\|Write`, one handler each with `if: Edit(*.cs)` / `if: Write(*.cs)` | `dotnet format whitespace --folder --include <file>` on the edited file only (about 2 s) | Never |
| `build-check.ps1` | `Stop` | If `*.cs`/project files changed since the last green build, runs `dotnet build -warnaserror -v q`. On failure exits 2 with the first 30 error lines so Claude fixes them before finishing. Honours `stop_hook_active`; the last green fingerprint is stored in `.git/claude-build-check` | Exit 2 on build failure |
| `qa-tests-only.ps1` | `PreToolUse` `Edit\|Write`, only while `qa-engineer` runs | Allows edits under `tests/` only | Exit 2 outside `tests/` |

All scripts **fail open**: an unexpected error writes a note to stderr and exits 0. Only deliberate policy decisions exit 2.

## Testing the hooks

```powershell
pwsh .claude/hooks/test-hooks.ps1
```

This pipes sample JSON payloads into every script and checks the exit codes (about 30 cases). Run it after changing any hook. To try one hook by hand:

```powershell
'{"tool_input":{"command":"git push --force"}}' | pwsh -NoProfile -File .claude/hooks/guard-bash.ps1; $LASTEXITCODE  # 2
```

Inside Claude Code, `/hooks` lists every configured hook and where it comes from.

## Disabling hooks locally

There is no per-hook switch. Choose one of these, and keep the change out of git:

- **All hooks, this project only**: add `{ "disableAllHooks": true }` to `.claude/settings.local.json` (gitignored).
- **All hooks, one session**: `claude --settings '{"disableAllHooks": true}'`.
- **One hook**: hooks from all settings files are merged, so `.claude/settings.local.json` cannot remove a single project hook. Delete its block from `.claude/settings.json` in your working copy and do not commit that change.
- **Stop build check only, temporarily**: commit or stash your C# changes, or let it run once green; it skips until build inputs change again.

## Design notes

- **Why PowerShell, not Bash + jq or C#?** One script set for Windows, macOS and Linux with built-in JSON parsing. Measured per call on Windows: `pwsh` about 0.4 s every time. A .NET 10 file-based app (`dotnet run hook.cs`) took 16 s on first run and after every edit (compilation), then about 0.3 s from cache. The cold compile would stall the first tool call after every clone or hook change, so PowerShell wins on predictability.
- **Why exit 2 instead of JSON decisions?** Exit 2 sends stderr straight to Claude as the reason, which is the simplest way to let it self-correct.
- Keep hooks fast and quiet on success: they run on every matching tool call.

## Suggested user-level notification hook (not committed)

Get a desktop notification when Claude needs input. Add to your **user** settings (`~/.claude/settings.json`), not to this project:

```json
{
  "hooks": {
    "Notification": [
      {
        "hooks": [
          {
            "type": "command",
            "command": "pwsh",
            "args": ["-NoProfile", "-Command", "Add-Type -AssemblyName System.Windows.Forms; $n = New-Object System.Windows.Forms.NotifyIcon; $n.Icon = [System.Drawing.SystemIcons]::Information; $n.Visible = $true; $n.ShowBalloonTip(5000, 'Claude Code', 'Claude needs your input', 'Info'); Start-Sleep 6; $n.Dispose()"],
            "async": true
          }
        ]
      }
    ]
  }
}
```

On macOS use `"command": "osascript", "args": ["-e", "display notification \"Claude needs your input\" with title \"Claude Code\""]`; on Linux use `"command": "notify-send", "args": ["Claude Code", "Claude needs your input"]`.
