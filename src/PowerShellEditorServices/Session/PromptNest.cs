using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.PowerShell.EditorServices.Utility;

namespace Microsoft.PowerShell.EditorServices.Session
{
    using System.Management.Automation;

    internal class PromptNest
    {
        private ConcurrentStack<NestFrame> _frameStack;
        private AsyncQueue<RunspaceHandle> _readLineQueue;
        private PowerShellContext _powerShellContext;

        internal PromptNest(PowerShellContext powerShellContext, PowerShell initialPowerShell)
        {
            _powerShellContext = powerShellContext;
            _readLineQueue = new AsyncQueue<RunspaceHandle>();
            _readLineQueue.EnqueueAsync(new RunspaceHandle(powerShellContext, true)).Wait();
            _frameStack = new ConcurrentStack<NestFrame>();
            _frameStack.Push(
                new NestFrame(
                    initialPowerShell,
                    NewHandleQueue()));
        }

        private NestFrame CurrentFrame
        {
            get
            {
                _frameStack.TryPeek(out NestFrame currentFrame);
                return currentFrame;
            }
        }

        internal void PushPromptContext()
        {
            _frameStack.Push(
                new NestFrame(
                    PowerShell.Create(RunspaceMode.CurrentRunspace),
                    NewHandleQueue()));
        }

        internal void PopPromptContext()
        {
            if (_frameStack.Count == 1)
            {
                return;
            }

            _frameStack.TryPop(out _);
        }

        internal PowerShell GetPowerShell()
        {
            return CurrentFrame.PowerShell;
        }

        internal async Task<RunspaceHandle> GetRunspaceHandle(bool isReadLine)
        {
            var mainHandle = await CurrentFrame.Queue.DequeueAsync();

            if (isReadLine)
            {
                return await _readLineQueue.DequeueAsync();
            }

            return mainHandle;
        }

        internal async Task ReleaseRunspaceHandle(RunspaceHandle runspaceHandle)
        {
            await CurrentFrame.Queue.EnqueueAsync(new RunspaceHandle(_powerShellContext, false));
            if (!runspaceHandle.IsReadLine)
            {
                return;
            }

            await _readLineQueue.EnqueueAsync(new RunspaceHandle(_powerShellContext, true));
        }

        internal bool IsMainThreadBusy()
        {
            return
                CurrentFrame.PowerShell.InvocationStateInfo.State == PSInvocationState.Running
                || CurrentFrame.Queue.IsEmpty;
        }

        internal bool IsReadLineBusy()
        {
            return _readLineQueue.IsEmpty;
        }

        internal int NestedPromptLevel => _frameStack.Count;

        private AsyncQueue<RunspaceHandle> NewHandleQueue()
        {
            var queue = new AsyncQueue<RunspaceHandle>();
            queue.EnqueueAsync(new RunspaceHandle(_powerShellContext)).Wait();
            return queue;
        }

        private class NestFrame
        {
            internal PowerShell PowerShell { get; }

            internal AsyncQueue<RunspaceHandle> Queue { get; }

            internal NestFrame(PowerShell powerShell, AsyncQueue<RunspaceHandle> handleQueue)
            {
                PowerShell = powerShell;
                Queue = handleQueue;
            }
        }
    }
}