//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation.Runspaces;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.PowerShell.EditorServices
{
    /// <summary>
    /// Provides a way to invoke PowerShell scripts in
    /// threads that run without a default runspace.
    /// </summary>
    public sealed class PowerShellTaskScheduler : TaskScheduler
    {
        [ThreadStatic] private static bool s_isRunning;
        private readonly BlockingCollection<Task> _taskQueue;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="PowerShellTaskScheduler"/> class.
        /// </summary>
        public PowerShellTaskScheduler()
        {
            _taskQueue = new BlockingCollection<Task>();

            string commandsModulePath = Path.Combine(
                Path.GetDirectoryName(
                    this.GetType().GetTypeInfo().Assembly.Location),
                @"..\..\Commands\PowerShellEditorServices.Commands.psd1");

            var initialState = InitialSessionState.CreateDefault2();
            initialState.ImportPSModule(new [] { commandsModulePath });

            Runspace = RunspaceFactory.CreateRunspace(initialState);

#if !CoreCLR
            Runspace.ApartmentState = ApartmentState.STA;
#endif

            Runspace.ThreadOptions = PSThreadOptions.ReuseThread;
            Runspace.Open();

            StartRunspaceThread();
        }

        /// <summary>
        /// Gets the dedicated runspace that is used to invoke
        /// background PowerShell scripts.
        /// </summary>
        internal Runspace Runspace { get; }

        /// <summary>
        /// Attempts to execute a task.
        /// </summary>
        /// <param name="task">The task to execute.</param>
        /// <param name="taskWasPreviouslyQueued">
        /// A value indicating whether the task was previously queued.
        /// </param>
        /// <returns>A value indicating whether the task was executed.</returns>
        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            if (taskWasPreviouslyQueued) return false;

            return s_isRunning && TryExecuteTask(task);
        }

        /// <summary>
        /// Gets an empty task array.
        /// </summary>
        /// <returns>An empty task array.</returns>
        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return new Task[] { };
        }

        /// <summary>
        /// Queues a task for later execution.
        /// </summary>
        /// <param name="task">The task to be queued.</param>
        protected override void QueueTask(Task task)
        {
            _taskQueue.Add(task);
        }

        private void StartRunspaceThread()
        {
            var thread = new Thread(RunOnRunspaceThread)
            {
                Name = "PSTaskScheduler Runspace Thread"
            };

            thread.Start();
        }

        private void RunOnRunspaceThread()
        {
            s_isRunning = true;
            bool shouldResetRunspace = false;

            try
            {
                if (Runspace.DefaultRunspace == null)
                {
                    shouldResetRunspace = true;
                    Runspace.DefaultRunspace = Runspace;
                }

                foreach (var task in _taskQueue.GetConsumingEnumerable())
                {
                    TryExecuteTask(task);
                }
            }
            finally
            {
                if (shouldResetRunspace)
                {
                    Runspace.DefaultRunspace = null;
                }

                s_isRunning = false;
            }
        }
    }
}
