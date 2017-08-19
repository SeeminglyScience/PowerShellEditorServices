//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Internal;
using Microsoft.PowerShell.EditorServices.Extensions;
using Microsoft.PowerShell.EditorServices.Utility;

namespace Microsoft.PowerShell.EditorServices
{
    /// <summary>
    /// Represents a collection of feature providers created from PowerShell.
    /// </summary>
    /// <typeparam name="TFeatureResult">
    /// The type of feature item returned by the provider.
    /// </typeparam>
    public abstract class PowerShellFeatureProvider<TFeatureResult> : PowerShellFeatureProviderBase
    {
        private readonly Dictionary<string, AnonymousFeatureProvider<TFeatureResult>> _providerStore;

        /// <summary>
        /// Initializes a new instances of the
        /// <see cref="PowerShellFeatureProvider"/> class.
        /// </summary>
        /// <param name="logger">The logger to use for exceptions.</param>
        public PowerShellFeatureProvider(ILogger logger) : base(logger)
        {
            _providerStore = new Dictionary<string, AnonymousFeatureProvider<TFeatureResult>>();
        }

        /// <summary>
        /// Gets the currently registered providers.
        /// </summary>
        public virtual IEnumerable<AnonymousFeatureProvider<TFeatureResult>> Providers
        {
            get
            {
                return new ReadOnlyCollection<AnonymousFeatureProvider<TFeatureResult>>(
                    new List<AnonymousFeatureProvider<TFeatureResult>>(
                        _providerStore.Values));
            }
        }

        /// <summary>
        /// Gets a specific registered provider.
        /// </summary>
        /// <param name="providerName">The name of the provider.</param>
        /// <returns>The matching provider or <see langword="null"/> if not found.</returns>
        public virtual AnonymousFeatureProvider<TFeatureResult> GetProvider(string providerName)
        {
            AnonymousFeatureProvider<TFeatureResult> provider;
            _providerStore.TryGetValue(providerName, out provider);

            return provider;
        }

        /// <summary>
        /// Registers a PowerShell script provider.
        /// </summary>
        /// <param name="providerName">The name of the provider.</param>
        /// <param name="providerAction">The action the provider invokes.</param>
        /// <returns>
        /// A value indicating if an existing provider was
        /// updated or if it was added as new.
        /// </returns>
        public virtual bool RegisterProvider(string providerName, ScriptBlock providerAction)
        {
            Validate.IsNotNullOrEmptyString(nameof(providerName), providerName);
            Validate.IsNotNull(nameof(providerAction), providerAction);

            return RegisterProvider(
                new AnonymousFeatureProvider<TFeatureResult>(
                    providerName,
                    providerAction,
                    Logger));
        }

        /// <summary>
        /// Registers a PowerShell script provider.
        /// </summary>
        /// <param name="provider">The provider to register.</param>
        /// <returns>
        /// A value indicating if an existing provider was
        /// updated or if it was added as new.
        /// </returns>
        public virtual bool RegisterProvider(AnonymousFeatureProvider<TFeatureResult> provider)
        {
            Validate.IsNotNull(nameof(provider), provider);

            provider.ParentProvider = this;
            if (_providerStore.ContainsKey(provider.ProviderId))
            {
                _providerStore[provider.ProviderId] = provider;
                return false;
            }

            _providerStore.Add(provider.ProviderId, provider);
            return true;
        }

        /// <summary>
        /// Unregisters a PowerShell script provider.
        /// </summary>
        /// <param name="providerName">The name of the provider to remove</param>
        /// <exception cref="KeyNotFound">
        /// <paramref name="providerName"/> is not registered.
        /// </exception>
        public virtual void UnregisterProvider(string providerName)
        {
            Validate.IsNotNullOrEmptyString(nameof(providerName), providerName);

            if (_providerStore.ContainsKey(providerName))
            {
                _providerStore.Remove(providerName);
                return;
            }

            throw new KeyNotFoundException(
                string.Format(
                    "Provider '{0}' is not registered",
                    providerName));
        }
    }
}
