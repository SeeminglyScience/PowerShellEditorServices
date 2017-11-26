using System.Threading;
using System.Threading.Tasks;
using Microsoft.PowerShell.EditorServices.Console;

namespace Microsoft.PowerShell.EditorServices.Session
{
    internal class LegacyReadLineContext : IPromptContext
    {
        private readonly ConsoleReadLine _legacyReadLine;

        internal LegacyReadLineContext(PowerShellContext powerShellContext)
        {
            _legacyReadLine = new ConsoleReadLine(powerShellContext);
        }

        public Task AbortReadLine()
        {
            return Task.FromResult(true);
        }

        public async Task<string> InvokeReadLine(bool isCommandLine, CancellationToken cancellationToken)
        {
            return await _legacyReadLine.InvokeLegacyReadLine(isCommandLine, cancellationToken);
        }

        public Task WaitForReadLineExit()
        {
            return Task.FromResult(true);
        }
    }
}