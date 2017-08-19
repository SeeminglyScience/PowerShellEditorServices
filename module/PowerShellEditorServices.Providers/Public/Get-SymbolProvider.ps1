#
# Copyright (c) Microsoft. All rights reserved.
# Licensed under the MIT license. See LICENSE file in the project root for full license information.
#

function Get-SymbolProvider {
    [CmdletBinding()]
    param(
        [string]
        $Name = '*'
    )
    end {
        $providerBase = GetPowerShellProvider -Feature Symbols

        $providerBase.GetProviders() |
            Where-Object ProviderId -Like $Name
    }
}
