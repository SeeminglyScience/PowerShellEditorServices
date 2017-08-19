#
# Copyright (c) Microsoft. All rights reserved.
# Licensed under the MIT license. See LICENSE file in the project root for full license information.
#

function Get-CodeLensProvider {
    [CmdletBinding()]
    param(
        [string]
        $Name = '*'
    )
    end {
        $providerBase = GetPowerShellProvider -Feature CodeLenses

        $providerBase.Providers |
            Where-Object ProviderId -Like $Name
    }
}
