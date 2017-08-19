#
# Copyright (c) Microsoft. All rights reserved.
# Licensed under the MIT license. See LICENSE file in the project root for full license information.
#

function GetPowerShellProvider {
    [OutputType([Microsoft.PowerShell.EditorServices.PowerShellFeatureProviderBase])]
    [CmdletBinding()]
    param(
        [ValidateSet('Symbols', 'CodeLenses')]
        [string]
        $Feature
    )
    end {

        $featureType = switch ($Feature) {
            Symbols { [Microsoft.PowerShell.EditorServices.Symbols.IDocumentSymbols] }
            CodeLenses { [Microsoft.PowerShell.EditorServices.CodeLenses.ICodeLenses] }
        }

        $powerShellFeatureBase = [Microsoft.PowerShell.EditorServices.PowerShellFeatureProviderBase]

        $provider = $psEditor.Components.Get($featureType).Providers |
            Where-Object { $PSItem -is $powerShellFeatureBase }

        if ($provider.Count -gt 1) { return $provider[0] }

        return $provider
    }
}
