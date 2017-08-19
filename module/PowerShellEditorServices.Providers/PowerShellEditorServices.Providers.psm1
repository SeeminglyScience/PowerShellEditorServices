#
# Copyright (c) Microsoft. All rights reserved.
# Licensed under the MIT license. See LICENSE file in the project root for full license information.
#

Get-ChildItem -Path $PSScriptRoot\Public\*.ps1, $PSScriptRoot\Private\*.ps1 | ForEach-Object {
    . $PSItem.FullName
}

Export-ModuleMember -Function *-*