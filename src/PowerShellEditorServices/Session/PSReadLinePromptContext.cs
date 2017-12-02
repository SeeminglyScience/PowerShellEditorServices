using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.PowerShell.EditorServices.Session {
    using System.Management.Automation;
    using Microsoft.PowerShell.EditorServices.Console;

    internal class PSReadLinePromptContext : IPromptContext {

        private const string READ_LINE_SCRIPT = @"
            [System.Diagnostics.DebuggerHidden()]
            [System.Diagnostics.DebuggerStepThrough()]
            param(
                [System.Threading.CancellationToken] $CancellationToken = [System.Threading.CancellationToken]::None,
                [runspace] $Runspace = $Host.Runspace,
                [System.Management.Automation.EngineIntrinsics] $EngineIntrinsics = $ExecutionContext
            )
            end {
                if ($CancellationToken.IsCancellationRequested) {
                    return [string]::Empty
                }

                return [Microsoft.PowerShell.PSConsoleReadLine]::ReadLine($Runspace, $EngineIntrinsics, $CancellationToken)
            }";

        private readonly PowerShellContext _powerShellContext;

        private PromptNest _promptNest;

        private InvocationEventQueue _invocationEventQueue;

        private ConsoleReadLine _consoleReadLine;

        private CancellationTokenSource _readLineCancellationSource;

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
            _readLineCancellationSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var localTokenSource = _readLineCancellationSource;
            if (localTokenSource.Token.IsCancellationRequested)
            {
                throw new TaskCanceledException();
            }

            try
            {
                if (!isCommandLine)
                {
                    return await _consoleReadLine.InvokeLegacyReadLine(
                        false,
                        _readLineCancellationSource.Token);
                }

                var result = (await _powerShellContext.ExecuteCommand<string>(
                    new PSCommand()
                        .AddScript(READ_LINE_SCRIPT)
                        .AddArgument(_readLineCancellationSource.Token),
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

                return cancellationToken.IsCancellationRequested
                    ? string.Empty
                    : result;
            }
            finally
            {
                _readLineCancellationSource = null;
            }
        }

        public async Task AbortReadLine() {
            if (_readLineCancellationSource == null)
            {
                return;
            }

            _readLineCancellationSource.Cancel();

            await WaitForReadLineExit();
        }

        public async Task WaitForReadLineExit () {
            using (await _promptNest.GetRunspaceHandle(true)) { }
        }
    }
}