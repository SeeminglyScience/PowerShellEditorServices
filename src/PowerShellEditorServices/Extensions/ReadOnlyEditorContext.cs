//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

namespace Microsoft.PowerShell.EditorServices.Extensions
{
    /// <summary>
    /// Represents a read only version of the editor context.
    /// </summary>
    public class ReadOnlyEditorContext : EditorContext
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="ReadOnlyEditorContext"/> class.
        /// </summary>
        /// <param name="scriptFile">
        /// The file that is the subject of this context.
        /// </param>
        public ReadOnlyEditorContext(ScriptFile scriptFile)
            : base(
                null,
                scriptFile,
                new BufferPosition(1, 1),
                new BufferRange(1, 1, 1, 1))
        {
            CurrentFile = new ReadOnlyFileContext(scriptFile, this);
        }

        /// <summary>
        /// Gets the current file.
        /// </summary>
        public new FileContext CurrentFile { get; }

        /// <summary>
        /// This method does not perform any action due to being read only.
        /// </summary>
        /// <param name="startLine"></param>
        /// <param name="startColumn"></param>
        /// <param name="endLine"></param>
        /// <param name="endColumn"></param>
        public new void SetSelection(int startLine, int startColumn, int endLine, int endColumn)
        { }

        /// <summary>
        /// This method does not perform any action due to being read only.
        /// </summary>
        /// <param name="startPosition"></param>
        /// <param name="endPosition"></param>
        public new void SetSelection(BufferPosition startPosition, BufferPosition endPosition)
        { }

        /// <summary>
        /// This method does not perform any action due to being read only.
        /// </summary>
        /// <param name="selectionRange"></param>
        public new void SetSelection(BufferRange selectionRange)
        { }
    }
}
