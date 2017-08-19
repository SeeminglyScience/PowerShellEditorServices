#
# Copyright (c) Microsoft. All rights reserved.
# Licensed under the MIT license. See LICENSE file in the project root for full license information.
#

function Unregister-CodeLensProvider {
    [CmdletBinding()]
    param(
        [Parameter(ValueFromPipeline)]
        [Alias('ProviderId')]
        [string]
        $Name
    )
    begin {
        $providerBase = GetPowerShellProvider -Feature CodeLenses
    }
    process {
        $providerBase.UnregisterProvider($Name)
    }
}
