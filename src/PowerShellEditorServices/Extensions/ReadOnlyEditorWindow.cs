//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

namespace Microsoft.PowerShell.EditorServices.Extensions
{
    /// <summary>
    /// Represents a read only version of the editor window.
    /// </summary>
    public class ReadOnlyEditorWindow : EditorWindow
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="ReadOnlyEditorWindow"/> class.
        /// </summary>
        public ReadOnlyEditorWindow() : base(null)
        { }

        /// <summary>
        /// This method does not perform any action due to being read only.
        /// </summary>
        /// <param name="message"></param>
        public new void SetStatusBarMessage(string message)
        { }

        /// <summary>
        /// This method does not perform any action due to being read only.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="timeout"></param>
        public new void SetStatusBarMessage(string message, int timeout)
        { }

        /// <summary>
        /// This method does not perform any action due to being read only.
        /// </summary>
        /// <param name="message"></param>
        public new void ShowErrorMessage(string message)
        { }

        /// <summary>
        /// This method does not perform any action due to being read only.
        /// </summary>
        /// <param name="message"></param>
        public new void ShowInformationMessage(string message)
        { }

        /// <summary>
        /// This method does not perform any action due to being read only.
        /// </summary>
        /// <param name="message"></param>
        public new void ShowWarningMessage(string message)
        { }
    }
}
