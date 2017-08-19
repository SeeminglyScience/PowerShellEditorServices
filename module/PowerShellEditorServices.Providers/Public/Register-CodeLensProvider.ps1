#
# Copyright (c) Microsoft. All rights reserved.
# Licensed under the MIT license. See LICENSE file in the project root for full license information.
#

function Register-CodeLensProvider {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [string]
        $Name,

        [Parameter(Mandatory)]
        [ValidateNotNull()]
        [scriptblock]
        $ScriptBlock
    )
    end {
        $providerBase = GetPowerShellProvider -Feature CodeLenses

        $null = $providerBase.RegisterProvider($Name, $ScriptBlock)
    }
}
