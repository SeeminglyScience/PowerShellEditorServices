using System;
using System.Collections.Generic;
using System.Management.Automation.Runspaces;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;

namespace Microsoft.PowerShell.EditorServices.Session
{
    internal class InvocationEventQueue
    {
        private readonly PromptNest _promptNest;
        private readonly Runspace _runspace;
        private readonly PowerShellContext _powerShellContext;
        private IPipelineExecutionRequest _pipelineRequest;

        internal InvocationEventQueue(PowerShellContext powerShellContext, PromptNest promptNest)
        {
            _promptNest = promptNest;
            _powerShellContext = powerShellContext;
            _runspace = powerShellContext.CurrentRunspace.Runspace;
            CreateInvocationSubscriber();
        }

        /// <summary>
        /// Executes a command on the main pipeline thread through
        /// eventing. A PSEngineEvent.OnIdle event subscriber will
        /// be created that creates a nested PowerShell instance for
        /// PowerShellContext.ExecuteCommand to utilize.
        /// </summary>
        /// <remarks>
        /// Avoid using this method directly if possible.
        /// PowerShellContext.ExecuteCommand will route commands
        /// through this method if required.
        /// </remarks>
        /// <typeparam name="TResult">The expected result type.</typeparam>
        /// <param name="psCommand">The PSCommand to be executed.</param>
        /// <param name="errorMessages">Error messages from PowerShell will be written to the StringBuilder.</param>
        /// <param name="executionOptions">Specifies options to be used when executing this command.</param>
        /// <returns>
        /// An awaitable Task which will provide results once the command
        /// execution completes.
        /// </returns>
        internal async Task<IEnumerable<TResult>> ExecuteCommandOnIdle<TResult>(
            PSCommand psCommand,
            StringBuilder errorMessages,
            ExecutionOptions executionOptions)
        {
            if (_pipelineRequest != null)
            {
                await _pipelineRequest.WaitTask;
            }

            var request = new PipelineExecutionRequest<TResult>(
                _powerShellContext,
                psCommand,
                errorMessages,
                executionOptions);

            _pipelineRequest = request;

            return await request.Results;
        }

        private void OnPowerShellIdle(object sender, EventArgs e)
        {
            if (_pipelineRequest == null || System.Console.KeyAvailable)
            {
                return;
            }

            _promptNest.PushPromptContext();
            try
            {
                _pipelineRequest.Execute().Wait();
            }
            finally
            {
                _promptNest.PopPromptContext();
                _pipelineRequest = null;
            }
        }

        private PSEventSubscriber CreateInvocationSubscriber()
        {
            PSEventSubscriber subscriber = _runspace.Events.SubscribeEvent(
                source: null,
                eventName: PSEngineEvent.OnIdle,
                sourceIdentifier: PSEngineEvent.OnIdle,
                data: null,
                handlerDelegate: OnPowerShellIdle,
                supportEvent: true,
                forwardEvent: false);

            SetSubscriberExecutionThreadWithReflection(subscriber);

            subscriber.Unsubscribed += OnInvokerUnsubscribed;

            return subscriber;
        }

        private void OnInvokerUnsubscribed(object sender, PSEventUnsubscribedEventArgs e)
        {
            CreateInvocationSubscriber();
        }

        private void SetSubscriberExecutionThreadWithReflection(PSEventSubscriber subscriber)
        {
            // We need to create the PowerShell object in the same thread so we can get a nested
            // PowerShell.  Without changes to PSReadLine directly, this is the only way to achieve
            // that consistently.  The alternative is to make the subscriber a script block and have
            // that create and process the PowerShell object, but that puts us in a different
            // SessionState and is a lot slower.

            // This should be safe as PSReadline should be waiting for pipeline input due to the
            // OnIdle event sent along with it.
            typeof(PSEventSubscriber)
                .GetProperty(
                    "ShouldProcessInExecutionThread",
                    BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(subscriber, true);
        }
    }
}