//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

using System.Collections.Generic;
using System.Linq;
using Microsoft.PowerShell.EditorServices.Utility;

namespace Microsoft.PowerShell.EditorServices.Symbols
{
    /// <summary>
    /// Represents a collection of symbol providers created from PowerShell.
    /// </summary>
    public class PowerShellDocumentSymbolProvider : PowerShellFeatureProvider<SymbolReference>, IDocumentSymbolProvider
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="PowerShellDocumentSymbolProvider"/> class.
        /// </summary>
        /// <param name="logger">The logger to use for exceptions.</param>
        public PowerShellDocumentSymbolProvider(ILogger logger) : base (logger)
        { }

        /// <summary>
        /// Invokes registered providers and returns matching symbols.
        /// </summary>
        /// <param name="scriptFile">
        /// The file to obtain symbol references for.
        /// </param>
        /// <returns>
        /// Matching symbols returned by registered providers.
        /// </returns>
        public IEnumerable<SymbolReference> ProvideDocumentSymbols(ScriptFile scriptFile)
        {
            return Providers
                .SelectMany(provider => provider.InvokeDefaultProviderAction(scriptFile));
        }
    }
}
