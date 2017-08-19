//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

using System.IO;
using System.Management.Automation;
using System.Management.Automation.Internal;
using Microsoft.PowerShell.EditorServices.Extensions;

namespace Microsoft.PowerShell.EditorServices.Extensions
{
    /// <summary>
    /// Represents a read only version of the editor object
    /// to allow PowerShell scripts to use $psEditor in
    /// requests to the language server.
    /// </summary>
    public class ReadOnlyEditorObject : EditorObject
    {
        private ScriptFile _scriptFile;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="ReadOnlyEditorObject"/> class.
        /// </summary>
        /// <param name="scriptFile">
        /// The file that is the subject of this object.
        /// </param>
        public ReadOnlyEditorObject(ScriptFile scriptFile) : base(null, null, null)
        {
            _scriptFile = scriptFile;
        }

        /// <summary>
        /// Gets a read only version of the editor window.
        /// </summary>
        public new EditorWindow Window
        {
            get
            {
                return new ReadOnlyEditorWindow();
            }
        }

        /// <summary>
        /// Gets a read only version of the editor workspace.
        /// </summary>
        public new EditorWorkspace Workspace
        {
            get
            {
                return new ReadOnlyEditorWorkspace(
                    Path.GetDirectoryName(
                        _scriptFile.FilePath));
            }
        }

        /// <summary>
        /// Creates a read only version of the editor context.
        /// </summary>
        /// <returns>A read only version of the editor context.</returns>
        public new ReadOnlyEditorContext GetEditorContext()
        {
            return new ReadOnlyEditorContext(_scriptFile);
        }

        /// <summary>
        /// This method does not perform any action due to being read only.
        /// </summary>
        /// <param name="editorCommand"></param>
        /// <returns>False</returns>
        public new bool RegisterCommand(EditorCommand editorCommand)
        {
            return false;
        }

        /// <summary>
        /// This method does not perform any action due to being read only.
        /// </summary>
        /// <param name="commandName"></param>
        public new void UnregisterCommand(string commandName)
        { }
    }
}
