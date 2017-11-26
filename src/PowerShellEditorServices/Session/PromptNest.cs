using Microsoft.PowerShell.EditorServices.Utility;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.PowerShell.EditorServices.Session
{
    using System.Management.Automation;

    internal class PromptNest
    {
        private Stack<AsyncQueue<RunspaceHandle>> _mainHandleStack;
        private Stack<PowerShell> _mainPowerShellStack;
        private AsyncQueue<RunspaceHandle> _readLineQueue;
        private PowerShellContext _powerShellContext;

        internal PromptNest(PowerShellContext powerShellContext, PowerShell initialPowerShell)
        {
            _powerShellContext = powerShellContext;
            _mainHandleStack = new Stack<AsyncQueue<RunspaceHandle>>();
            _mainPowerShellStack = new Stack<PowerShell>();
            _readLineQueue = new AsyncQueue<RunspaceHandle>();
            _mainHandleStack.Push(NewHandleQueue());
            _mainPowerShellStack.Push(initialPowerShell);
            _readLineQueue.EnqueueAsync(new RunspaceHandle(powerShellContext, true)).Wait();
        }

        internal void PushPromptContext()
        {
            _mainPowerShellStack.Push(PowerShell.Create(RunspaceMode.CurrentRunspace));
            _mainHandleStack.Push(NewHandleQueue());
        }

        internal void PopPromptContext()
        {
            if (_mainHandleStack.Count == 1)
            {
                return;
            }

            _mainPowerShellStack.Pop().Dispose();
            _mainHandleStack.Pop();
        }

        internal PowerShell GetPowerShell()
        {
            return _mainPowerShellStack.Peek();
        }

        internal async Task<RunspaceHandle> GetRunspaceHandle(bool isReadLine)
        {
            var mainHandle = await _mainHandleStack.Peek().DequeueAsync();

            if (isReadLine)
            {
                return await _readLineQueue.DequeueAsync();
            }

            return mainHandle;
        }

        internal async Task ReleaseRunspaceHandle(RunspaceHandle runspaceHandle)
        {
            await _mainHandleStack.Peek().EnqueueAsync(new RunspaceHandle(_powerShellContext, false));
            if (!runspaceHandle.IsReadLine)
            {
                return;
            }

            await _readLineQueue.EnqueueAsync(new RunspaceHandle(_powerShellContext, true));
        }

        internal bool IsMainThreadBusy()
        {
            return
                _mainPowerShellStack.Peek().InvocationStateInfo.State == PSInvocationState.Running
                || _mainHandleStack.Peek().IsEmpty;
        }

        internal bool IsReadLineBusy()
        {
            return _readLineQueue.IsEmpty;
        }

        internal int NestedPromptLevel => _mainHandleStack.Count;

        private AsyncQueue<RunspaceHandle> NewHandleQueue()
        {
            var queue = new AsyncQueue<RunspaceHandle>();
            queue.EnqueueAsync(new RunspaceHandle(_powerShellContext)).Wait();
            return queue;
        }
    }
}