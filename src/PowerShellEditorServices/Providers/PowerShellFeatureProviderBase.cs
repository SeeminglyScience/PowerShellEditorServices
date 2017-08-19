//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

using Microsoft.PowerShell.EditorServices.Utility;

namespace Microsoft.PowerShell.EditorServices
{
    /// <summary>
    /// Represents a feature provider that has a
    /// dedicated runspace for custom provider actions.
    /// </summary>
    public abstract class PowerShellFeatureProviderBase : FeatureProviderBase
    {
        private static PowerShellTaskScheduler s_scheduler;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="PowerShellFeatureProviderBase"/> class.
        /// </summary>
        /// <param name="logger">The logger to use for exceptions.</param>
        public PowerShellFeatureProviderBase(ILogger logger)
        {
            Logger = logger;
        }

        /// <summary>
        /// Gets the default task scheduler for custom providers.
        /// </summary>
        public static PowerShellTaskScheduler DefaultScheduler
        {
            get
            {
                if (s_scheduler == null)
                {
                    s_scheduler = new PowerShellTaskScheduler();
                }

                return s_scheduler;
            }
        }

        /// <summary>
        /// Gets the logger to use for exceptions.
        /// </summary>
        protected ILogger Logger { get; }
    }
}
