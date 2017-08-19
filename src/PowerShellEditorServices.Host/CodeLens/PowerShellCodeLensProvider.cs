//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.PowerShell.EditorServices.Utility;

namespace Microsoft.PowerShell.EditorServices.CodeLenses
{
    /// <summary>
    /// Represents a collection of code lens providers created from PowerShell.
    /// </summary>
    public class PowerShellCodeLensProvider : PowerShellFeatureProvider<CodeLens>, ICodeLensProvider
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="PowerShellCodeLensProvider"/> class.
        /// </summary>
        /// <param name="logger">The logger to use for exceptions.</param>
        public PowerShellCodeLensProvider(ILogger logger) : base(logger)
        { }

        /// <summary>
        /// Invokes registered providers and returns matching code lenses.
        /// </summary>
        /// <param name="scriptFile">The file to obtain code lenses for.</param>
        /// <returns>Matching code lenses returned by registered providers.</returns>
        public CodeLens[] ProvideCodeLenses(ScriptFile scriptFile)
        {
            return Providers
                .SelectMany(provider => provider.InvokeDefaultProviderAction(scriptFile))
                .ToArray();
        }

        /// <summary>
        /// Returns the code lens as is as this is
        /// currently unsupported for custom providers.
        /// </summary>
        /// <param name="codeLens">The code lens to resolve.</param>
        /// <param name="cancellationToken">A token that can be used to cancel the result.</param>
        /// <returns>The resolved code lens.</returns>
        public Task<CodeLens> ResolveCodeLensAsync(CodeLens codeLens, CancellationToken cancellationToken)
        {
            return Task.FromResult(codeLens);
        }
    }
}
