//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

using Microsoft.PowerShell.EditorServices;

namespace Microsoft.PowerShell.EditorServices.Extensions
{
    /// <summary>
    /// Represents a read only version of file context.
    /// </summary>
    public class ReadOnlyFileContext : FileContext
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="ReadOnlyFileContext"/> class.
        /// </summary>
        /// <param name="scriptFile">The file to obtain context from.</param>
        /// <param name="editorContext">The parent editor context.</param>
        public ReadOnlyFileContext(
            ScriptFile scriptFile,
            ReadOnlyEditorContext editorContext)
            : base(
                scriptFile,
                editorContext,
                null)
        { }

        /// <summary>
        /// This method does not perform any action due to being read only.
        /// </summary>
        /// <param name="textToInsert"></param>
        public new void InsertText(string textToInsert)
        { }

        /// <summary>
        /// This method does not perform any action due to being read only.
        /// </summary>
        /// <param name="textToInsert"></param>
        /// <param name="insertPosition"></param>
        public new void InsertText(string textToInsert, BufferPosition insertPosition)
        { }

        /// <summary>
        /// This method does not perform any action due to being read only.
        /// </summary>
        /// <param name="textToInsert"></param>
        /// <param name="insertLine"></param>
        /// <param name="insertColumn"></param>
        public new void InsertText(string textToInsert, int insertLine, int insertColumn)
        { }

        /// <summary>
        /// This method does not perform any action due to being read only.
        /// </summary>
        /// <param name="textToInsert"></param>
        /// <param name="startLine"></param>
        /// <param name="startColumn"></param>
        /// <param name="endLine"></param>
        /// <param name="endColumn"></param>
        public new void InsertText(string textToInsert, int startLine, int startColumn, int endLine, int endColumn)
        { }

        /// <summary>
        /// This method does not perform any action due to being read only.
        /// </summary>
        /// <param name="textToInsert"></param>
        /// <param name="insertRange"></param>
        public new void InsertText(string textToInsert, BufferRange insertRange)
        { }
    }
}
