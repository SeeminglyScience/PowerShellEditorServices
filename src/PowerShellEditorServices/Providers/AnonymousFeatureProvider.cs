//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Language;
using System.Management.Automation.Runspaces;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.PowerShell.EditorServices.Extensions;
using Microsoft.PowerShell.EditorServices.Utility;

namespace Microsoft.PowerShell.EditorServices
{
    /// <summary>
    /// Represents a custom provider that invokes a PowerShell script.
    /// </summary>
    /// <typeparam name="TFeatureResult">
    /// The type of feature item returned by the provider.
    /// </typeparam>
    public class AnonymousFeatureProvider<TFeatureResult> : FeatureProviderBase
    {
        /// <summary>
        /// Represents a custom provider action that wraps a PowerShell script.
        /// </summary>
        /// <param name="scriptFile">The file to obtain feature results for.</param>
        /// <param name="provider">The provider that is invoking this action.</param>
        /// <returns>The output of the PowerShell script.</returns>
        public delegate TFeatureResult[] ProviderAction(
            ScriptFile scriptFile,
            AnonymousFeatureProvider<TFeatureResult> provider);

        private readonly PowerShellTaskScheduler _scheduler;

        private readonly ILogger _logger;

        private readonly ProviderAction _defaultProviderAction;


        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="AnonymousFeatureProvider"/> class.
        /// </summary>
        /// <param name="name">The unique identifier for this provider.</param>
        /// <param name="defaultProviderAction">The action this provider invokes.</param>
        /// <param name="logger">The logger to use for exceptions.</param>
        public AnonymousFeatureProvider(
            string name,
            ScriptBlock defaultProviderAction,
            ILogger logger)
                : this(
                    name,
                    defaultProviderAction,
                    logger,
                    PowerShellFeatureProvider<TFeatureResult>.DefaultScheduler) { }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="AnonymousFeatureProvider"/> class.
        /// </summary>
        /// <param name="name">The unique identifier for this provider.</param>
        /// <param name="defaultProviderAction">The action this provider invokes.</param>
        /// <param name="logger">The logger for exceptions.</param>
        /// <param name="scheduler">The PowerShell task scheduler to use for invocation.</param>
        public AnonymousFeatureProvider(
            string name,
            ScriptBlock defaultProviderAction,
            ILogger logger,
            PowerShellTaskScheduler scheduler)
        {
            Validate.IsNotNullOrEmptyString(nameof(name), name);
            Validate.IsNotNull(nameof(defaultProviderAction), defaultProviderAction);
            Validate.IsNotNull(nameof(scheduler), scheduler);

            ProviderId = name;
            _logger = logger;
            _scheduler = scheduler;
            _defaultProviderAction = WrapScriptBlock(defaultProviderAction);
        }

        /// <summary>
        /// Gets the name of this provider.
        /// </summary>
        public new string ProviderId { get; }

        /// <summary>
        /// Gets or sets the PowerShell feature provider this object belongs to.
        /// </summary>
        public PowerShellFeatureProvider<TFeatureResult> ParentProvider { get; set; }

        /// <summary>
        /// Invokes the default action for the provider
        /// (e.g. ProvideSymbols, ProvideCodeLens, etc)
        /// </summary>
        /// <param name="scriptFile">The file to obtain feature results for.</param>
        /// <returns>The output of the PowerShell script.</returns>
        public IEnumerable<TFeatureResult> InvokeDefaultProviderAction(ScriptFile scriptFile)
        {
            Validate.IsNotNull(nameof(scriptFile), scriptFile);

            // Invoke the provider action in a dedicated thread to ensure a dedicated default runspace
            // exists even when invoked within tasks.
            var task = Task<IEnumerable<TFeatureResult>>.Factory.StartNew(
                () =>
                {
                    // Set $psEditor to a read only version that cannot make requests to the
                    // language service. Without this the thread hangs on GetEditorContext.
                    Runspace
                        .DefaultRunspace
                        .SessionStateProxy
                        .SetVariable(
                            "psEditor",
                            new ReadOnlyEditorObject(scriptFile));

                    return _defaultProviderAction(scriptFile, this);
                },
                new CancellationToken(false),
                TaskCreationOptions.None,
                _scheduler);
            try
            {
                return task.Result ?? Enumerable.Empty<TFeatureResult>();
            }
            catch (AggregateException exception)
            {
                // Check if we can find a exception that contains an error record first so we can
                // log invocation info.
                var runtimeException = exception
                    .InnerExceptions
                    .OfType<RuntimeException>()
                    .FirstOrDefault();
                if (runtimeException != null)
                {
                    _logger.WriteException(
                        runtimeException.Message,
                        runtimeException,
                        ProviderId,
                        runtimeException.ErrorRecord.InvocationInfo?.ScriptName ?? string.Empty,
                        runtimeException.ErrorRecord.InvocationInfo?.ScriptLineNumber ?? 0);

                    return Enumerable.Empty<TFeatureResult>();
                }

                var invalidCastException = exception
                    .InnerExceptions
                    .OfType<PSInvalidCastException>()
                    .FirstOrDefault();
                if (invalidCastException != null)
                {
                    _logger.WriteException(
                        invalidCastException.Message,
                        invalidCastException,
                        ProviderId,
                        invalidCastException.ErrorRecord.InvocationInfo?.ScriptName ?? string.Empty,
                        invalidCastException.ErrorRecord.InvocationInfo?.ScriptLineNumber ?? 0);

                    return Enumerable.Empty<TFeatureResult>();
                }

                // Catch all other exceptions otherwise the background async loop will fail and
                // the integrated console will crash.
                _logger.WriteException(
                    exception.Message,
                    exception,
                    ProviderId);

                return Enumerable.Empty<TFeatureResult>();
            }
        }

        /// <summary>
        /// Wraps a PowerShell script in a <see cref="ProviderAction"/> delegate so it can
        /// be invoked in an asynchronous context.
        /// </summary>
        /// <param name="scriptBlock">The script to wrap.</param>
        /// <returns>A delegate that invokes the script.</returns>
        protected ProviderAction WrapScriptBlock(ScriptBlock scriptBlock)
        {
            // In PS 5.1 ScriptBlocks are marshaled to their originating runspace, so we have to dump
            // execution context by recreating it.  This method of recreating the ScriptBlock keeps
            // invocation info (file, extent in file, etc)
            ScriptBlockAst body = scriptBlock.Ast as ScriptBlockAst;

            if (body == null)
            {
                body = scriptBlock
                    .Ast
                    .Find(
                        ast => ast.GetType() == typeof(ScriptBlockAst),
                        true)
                        as ScriptBlockAst;
            }
            return LanguagePrimitives.ConvertTo<ProviderAction>(body.GetScriptBlock());
        }
    }
}
