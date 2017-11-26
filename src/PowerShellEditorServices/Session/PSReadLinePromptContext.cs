using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.PowerShell.EditorServices.Session {
    using System.Management.Automation;
    using Microsoft.PowerShell.EditorServices.Console;

    internal class PSReadLinePromptContext : IPromptContext {

        // The current way of cancelling PSReadLine is very hacky and
        // definitely needs to be addressed before releasing.
        private const string CANCEL_READ_LINE_SCRIPT = @"

            [System.Diagnostics.DebuggerHidden()]
            [System.Diagnostics.DebuggerStepThrough()]
            param()
            end {
                $commandText = $null
                [Microsoft.PowerShell.PSConsoleReadLine]::GetBufferState(
                    [ref]$commandText,
                    [ref]$null)

                [Microsoft.PowerShell.PSConsoleReadLine]::RevertLine()
                if (-not [string]::IsNullOrEmpty($commandText)) {
                    [Microsoft.PowerShell.PSConsoleReadLine]::AddToHistory($commandText)
                }

                $singleton = [Microsoft.PowerShell.PSConsoleReadline].
                    GetField('_singleton', [System.Reflection.BindingFlags]'Static, NonPublic').
                    GetValue($null)

                $key = [System.ConsoleKeyInfo]::new(
                    [char]13,
                    [System.ConsoleKey]::Enter,
                    $false,
                    $false,
                    $false)

                [Microsoft.PowerShell.PSConsoleReadLine].
                    GetField('_queuedKeys', [System.Reflection.BindingFlags]'Instance, NonPublic').
                    GetValue($singleton).
                    Enqueue($key)

                $null = [Microsoft.PowerShell.PSConsoleReadline].
                    GetField('_keyReadWaitHandle', [System.Reflection.BindingFlags]'Instance, NonPublic').
                    GetValue($singleton).
                    Set()
            }";

        private const string READ_LINE_SCRIPT = @"
            [System.Diagnostics.DebuggerHidden()]
            [System.Diagnostics.DebuggerStepThrough()]
            param(
                [runspace] $Runspace,
                [System.Management.Automation.EngineIntrinsics] $EngineIntrinsics
            )
            end {
                if ($Runspace -and $EngineIntrinsics) {
                    return [Microsoft.PowerShell.PSConsoleReadLine]::ReadLine($Runspace, $EngineIntrinsics)
                }

                return PSConsoleHostReadline
            }";

        private readonly PowerShellContext _powerShellContext;

        private PromptNest _promptNest;

        private InvocationEventQueue _invocationEventQueue;

        private ConsoleReadLine _consoleReadLine;

        internal PSReadLinePromptContext(
            PowerShellContext powerShellContext,
            PromptNest promptNest,
            InvocationEventQueue invocationEventQueue)
        {
            _promptNest = promptNest;
            _powerShellContext = powerShellContext;
            _invocationEventQueue = invocationEventQueue;
            _consoleReadLine = new ConsoleReadLine(powerShellContext);
        }

        public async Task<string> InvokeReadLine(bool isCommandLine, CancellationToken cancellationToken)
        {
            if (!isCommandLine)
            {
                return await _consoleReadLine.InvokeLegacyReadLine(false, cancellationToken);
            }

            if (_promptNest.IsReadLineBusy())
            {
                await AbortReadLine();
            }

            cancellationToken.Register(OnReadLineCancelled);

            return (await _powerShellContext.ExecuteCommand<string>(
                new PSCommand().AddScript(READ_LINE_SCRIPT),
                null,
                new ExecutionOptions()
                {
                    WriteErrorsToHost = false,
                    WriteOutputToHost = false,
                    InterruptCommandPrompt = false,
                    AddToHistory = false,
                    IsReadLine = isCommandLine
                }))
                .FirstOrDefault();
        }

        public async Task AbortReadLine() {
            if (!_promptNest.IsReadLineBusy())
            {
                return;
            }

            await _invocationEventQueue.ExecuteCommandOnIdle<PSObject> (
                new PSCommand().AddScript(CANCEL_READ_LINE_SCRIPT),
                null,
                new ExecutionOptions() {
                    WriteErrorsToHost = false,
                        WriteOutputToHost = false,
                        AddToHistory = false
                });
        }

        public async Task WaitForReadLineExit () {
            using (await _promptNest.GetRunspaceHandle(true)) { }
        }

        private async void OnReadLineCancelled () {
            if (!_promptNest.IsReadLineBusy())
            {
                return;
            }

            await AbortReadLine();
        }
    }
}