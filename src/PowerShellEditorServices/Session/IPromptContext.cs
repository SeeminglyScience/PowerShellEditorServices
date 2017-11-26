using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.PowerShell.EditorServices.Session
{
    public interface IPromptContext
    {
        Task<string> InvokeReadLine(bool isCommandLine, CancellationToken cancellationToken);

        Task AbortReadLine();

        Task WaitForReadLineExit();
    }
}