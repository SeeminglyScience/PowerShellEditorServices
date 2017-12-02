using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.PowerShell.EditorServices.Utility;

namespace Microsoft.PowerShell.EditorServices.Session
{
    using System.Management.Automation;

    internal class PromptNest
    {
        private ConcurrentStack<NestFrame> _frameStack;
        private NestFrame _readLineFrame;
        private PowerShellContext _powerShellContext;

        internal PromptNest(PowerShellContext powerShellContext, PowerShell initialPowerShell)
        {
            _powerShellContext = powerShellContext;
            _frameStack = new ConcurrentStack<NestFrame>();
            _frameStack.Push(
                new NestFrame(
                    initialPowerShell,
                    NewHandleQueue(),
                    false));

            var readLineShell = PowerShell.Create();
            readLineShell.Runspace = powerShellContext.CurrentRunspace.Runspace;
            _readLineFrame = new NestFrame(
                readLineShell,
                new AsyncQueue<RunspaceHandle>(),
                false);

            ReleaseRunspaceHandleImpl(true).Wait();
        }

        internal bool IsInDebugger
        {
            get
            {
                return CurrentFrame.IsDebugger;
            }
        }

        private NestFrame CurrentFrame
        {
            get
            {
                _frameStack.TryPeek(out NestFrame currentFrame);
                return currentFrame;
            }
        }

        internal void PushPromptContext(bool isDebugger = false)
        {
            _frameStack.Push(
                new NestFrame(
                    PowerShell.Create(RunspaceMode.CurrentRunspace),
                    NewHandleQueue(),
                    isDebugger));
        }

        internal void PopPromptContext()
        {
            if (_frameStack.Count == 1)
            {
                return;
            }

            _frameStack.TryPop(out _);
        }

        internal PowerShell GetPowerShell(bool isReadLine = false)
        {
            if (_frameStack.Count > 1)
            {
                return CurrentFrame.PowerShell;
            }

            return isReadLine ? _readLineFrame.PowerShell : CurrentFrame.PowerShell;
        }

        internal async Task<RunspaceHandle> GetRunspaceHandle(bool isReadLine)
        {
            // Also grab the main runspace handle if this is for a ReadLine pipeline and the runspace
            // is in process.
            if (isReadLine && !_powerShellContext.IsCurrentRunspaceOutOfProcess())
            {
                await GetRunspaceHandleImpl(false);
            }

            return await GetRunspaceHandleImpl(isReadLine);
        }

        internal async Task ReleaseRunspaceHandle(RunspaceHandle runspaceHandle)
        {
            await ReleaseRunspaceHandleImpl(runspaceHandle.IsReadLine);
            if (runspaceHandle.IsReadLine && !_powerShellContext.IsCurrentRunspaceOutOfProcess())
            {
                await ReleaseRunspaceHandleImpl(false);
            }
        }

        internal bool IsMainThreadBusy()
        {
            return CurrentFrame.Queue.IsEmpty;
        }

        internal bool IsReadLineBusy()
        {
            return _readLineFrame.Queue.IsEmpty;
        }

        internal int NestedPromptLevel => _frameStack.Count;

        private AsyncQueue<RunspaceHandle> NewHandleQueue()
        {
            var queue = new AsyncQueue<RunspaceHandle>();
            queue.EnqueueAsync(new RunspaceHandle(_powerShellContext)).Wait();
            return queue;
        }

        private async Task<RunspaceHandle> GetRunspaceHandleImpl(bool isReadLine)
        {
            if (isReadLine)
            {
                return await _readLineFrame.Queue.DequeueAsync();
            }

            return await CurrentFrame.Queue.DequeueAsync();
        }

        private async Task ReleaseRunspaceHandleImpl(bool isReadLine)
        {
            if (isReadLine)
            {
                await _readLineFrame.Queue.EnqueueAsync(new RunspaceHandle(_powerShellContext, true));
                return;
            }

            await CurrentFrame.Queue.EnqueueAsync(new RunspaceHandle(_powerShellContext, false));
        }

        private class NestFrame
        {
            internal PowerShell PowerShell { get; }

            internal AsyncQueue<RunspaceHandle> Queue { get; }

            internal bool IsDebugger { get; }

            internal NestFrame(
                PowerShell powerShell,
                AsyncQueue<RunspaceHandle> handleQueue,
                bool isDebugger)
            {
                PowerShell = powerShell;
                Queue = handleQueue;
                IsDebugger = isDebugger;
            }
        }
    }
}