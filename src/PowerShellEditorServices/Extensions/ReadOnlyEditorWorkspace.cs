//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

namespace Microsoft.PowerShell.EditorServices.Extensions
{
    /// <summary>
    /// Represents a read only version fo the editor workspace.
    /// </summary>
    public class ReadOnlyEditorWorkspace : EditorWorkspace
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="ReadOnlyEditorWorkspace"/> class.
        /// </summary>
        /// <param name="workspacePath">The path to display as the workspace path.</param>
        public ReadOnlyEditorWorkspace(string workspacePath) : base(null)
        {
            Path = workspacePath;
        }

        /// <summary>
        /// Gets the workspace path.
        /// </summary>
        public new string Path { get; }

        /// <summary>
        /// This method does not perform any action due to being read only.
        /// </summary>
        public new void NewFile()
        { }

        /// <summary>
        /// This method does not perform any action due to being read only.
        /// </summary>
        /// <param name="filePath"></param>
        public new void OpenFile(string filePath)
        { }
    }
}
