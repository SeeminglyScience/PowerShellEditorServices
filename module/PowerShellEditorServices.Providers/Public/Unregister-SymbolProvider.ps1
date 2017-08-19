#
# Copyright (c) Microsoft. All rights reserved.
# Licensed under the MIT license. See LICENSE file in the project root for full license information.
#

function Unregister-SymbolProvider {
    [CmdletBinding()]
    param(
        [Parameter(ValueFromPipeline, ValueFromPipelineByPropertyName)]
        [Alias('ProviderId')]
        [string]
        $Name
    )
    begin {
        $providerBase = GetPowerShellProvider -Feature Symbols
    }
    process {
        $providerBase.UnregisterProvider($Name)
    }
}
