//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.PowerShell.EditorServices.Console
{
    /// <summary>
    /// Provides platform specific console utilities.
    /// </summary>
    public interface IConsoleOperations
    {
        /// <summary>
        /// Obtains the next character or function key pressed by the user asynchronously.
        /// Does not block when other console API's are called.
        /// </summary>
        /// <param name="cancellationToken">The CancellationToken to observe.</param>
        /// <returns>
        /// A task that will complete with a result of the key pressed by the user.
        /// </returns>
        Task<ConsoleKeyInfo> ReadKeyAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Obtains the horizontal position of the console cursor. Use this method
        /// instead of <see cref="System.Console.CursorLeft" /> to avoid triggering
        /// pending calls to <see cref="IConsoleOperations.ReadKeyAsync(CancellationToken)" />
        /// on Unix platforms.
        /// </summary>
        /// <returns>The horizontal position of the console cursor.</returns>
        int GetCursorLeft();

        /// <summary>
        /// Obtains the horizontal position of the console cursor. Use this method
        /// instead of <see cref="System.Console.CursorLeft" /> to avoid triggering
        /// pending calls to <see cref="IConsoleOperations.ReadKeyAsync(CancellationToken)" />
        /// on Unix platforms.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> to observe.</param>
        /// <returns>The horizontal position of the console cursor.</returns>
        int GetCursorLeft(CancellationToken cancellationToken);

        /// <summary>
        /// Sets the Cursor's Position. Use this method instead of
        /// <see cref="System.Console.SetCursorPosition" /> to avoid triggering pending calls
        /// to <see cref="IConsoleOperations.ReadKeyAsync(CancellationToken)" />
        /// on Unix platforms.
        /// </summary>
        /// <param name="left">The column where the cursor should go.</param>
        /// <param name="top">The row where the cursor should go.</param>
        void SetCursorPosition(int left, int top);

        /// <summary>
        /// Sets the Cursor's Position. Use this method instead of
        /// <see cref="System.Console.SetCursorPosition" /> to avoid triggering pending calls
        /// to <see cref="IConsoleOperations.ReadKeyAsync(CancellationToken)" />
        /// on Unix platforms.
        /// </summary>
        /// <param name="left">The column where the cursor should go.</param>
        /// <param name="top">The row where the cursor should go.</param>
        Task SetCursorPositionAsync(int left, int top);

        /// <summary>
        /// Sets the Cursor's Position. Use this method instead of
        /// <see cref="System.Console.SetCursorPosition" /> to avoid triggering pending calls
        /// to <see cref="IConsoleOperations.ReadKeyAsync(CancellationToken)" />
        /// on Unix platforms.
        /// </summary>
        /// <param name="left">The column where the cursor should go.</param>
        /// <param name="top">The row where the cursor should go.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> to observe.</param>
        void SetCursorPosition(int left, int top, CancellationToken cancellationToken);

        /// <summary>
        /// Sets the Cursor's Position. Use this method instead of
        /// <see cref="System.Console.SetCursorPosition" /> to avoid triggering pending calls
        /// to <see cref="IConsoleOperations.ReadKeyAsync(CancellationToken)" />
        /// on Unix platforms.
        /// </summary>
        /// <param name="left">The column where the cursor should go.</param>
        /// <param name="top">The row where the cursor should go.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> to observe.</param>
        Task SetCursorPositionAsync(int left, int top, CancellationToken cancellationToken);

        /// <summary>
        /// Writes to stdout. Use this method instead of
        /// <see cref="System.Console.Write" /> to avoid triggering pending calls
        /// to <see cref="IConsoleOperations.ReadKeyAsync(CancellationToken)" />
        /// on Unix platforms.
        /// </summary>
        /// <param name="value">The character to write.</param>
        void Write(char value);

        /// <summary>
        /// Writes to stdout. Use this method instead of
        /// <see cref="System.Console.Write" /> to avoid triggering pending calls
        /// to <see cref="IConsoleOperations.ReadKeyAsync(CancellationToken)" />
        /// on Unix platforms.
        /// </summary>
        /// <param name="value">The character to write.</param>
        Task WriteAsync(char value);

        /// <summary>
        /// Writes to stdout. Use this method instead of
        /// <see cref="System.Console.Write" /> to avoid triggering pending calls
        /// to <see cref="IConsoleOperations.ReadKeyAsync(CancellationToken)" />
        /// on Unix platforms.
        /// </summary>
        /// <param name="value">The character to write.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> to observe.</param>
        void Write(char value, CancellationToken cancellationToken);

        /// <summary>
        /// Writes to stdout. Use this method instead of
        /// <see cref="System.Console.Write" /> to avoid triggering pending calls
        /// to <see cref="IConsoleOperations.ReadKeyAsync(CancellationToken)" />
        /// on Unix platforms.
        /// </summary>
        /// <param name="value">The character to write.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> to observe.</param>
        Task WriteAsync(char value, CancellationToken cancellationToken);

        /// <summary>
        /// Writes to stdout. Use this method instead of
        /// <see cref="System.Console.Write" /> to avoid triggering pending calls
        /// to <see cref="IConsoleOperations.ReadKeyAsync(CancellationToken)" />
        /// on Unix platforms.
        /// </summary>
        /// <param name="value">The string to write.</param>
        void Write(string value);

        /// <summary>
        /// Writes to stdout. Use this method instead of
        /// <see cref="System.Console.Write" /> to avoid triggering pending calls
        /// to <see cref="IConsoleOperations.ReadKeyAsync(CancellationToken)" />
        /// on Unix platforms.
        /// </summary>
        /// <param name="value">The string to write.</param>
        Task WriteAsync(string value);

        /// <summary>
        /// Writes to stdout. Use this method instead of
        /// <see cref="System.Console.Write" /> to avoid triggering pending calls
        /// to <see cref="IConsoleOperations.ReadKeyAsync(CancellationToken)" />
        /// on Unix platforms.
        /// </summary>
        /// <param name="value">The string to write.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> to observe.</param>
        void Write(string value, CancellationToken cancellationToken);

        /// <summary>
        /// Writes to stdout. Use this method instead of
        /// <see cref="System.Console.Write" /> to avoid triggering pending calls
        /// to <see cref="IConsoleOperations.ReadKeyAsync(CancellationToken)" />
        /// on Unix platforms.
        /// </summary>
        /// <param name="value">The string to write.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> to observe.</param>
        Task WriteAsync(string value, CancellationToken cancellationToken);

        /// <summary>
        /// Obtains the horizontal position of the console cursor. Use this method
        /// instead of <see cref="System.Console.CursorLeft" /> to avoid triggering
        /// pending calls to <see cref="IConsoleOperations.ReadKeyAsync(CancellationToken)" />
        /// on Unix platforms.
        /// </summary>
        /// <returns>
        /// A <see cref="Task{T}" /> representing the asynchronous operation. The
        /// <see cref="Task{T}.Result" /> property will return the horizontal position
        /// of the console cursor.
        /// </returns>
        Task<int> GetCursorLeftAsync();

        /// <summary>
        /// Obtains the horizontal position of the console cursor. Use this method
        /// instead of <see cref="System.Console.CursorLeft" /> to avoid triggering
        /// pending calls to <see cref="IConsoleOperations.ReadKeyAsync(CancellationToken)" />
        /// on Unix platforms.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> to observe.</param>
        /// <returns>
        /// A <see cref="Task{T}" /> representing the asynchronous operation. The
        /// <see cref="Task{T}.Result" /> property will return the horizontal position
        /// of the console cursor.
        /// </returns>
        Task<int> GetCursorLeftAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Obtains the vertical position of the console cursor. Use this method
        /// instead of <see cref="System.Console.CursorTop" /> to avoid triggering
        /// pending calls to <see cref="IConsoleOperations.ReadKeyAsync(CancellationToken)" />
        /// on Unix platforms.
        /// </summary>
        /// <returns>The vertical position of the console cursor.</returns>
        int GetCursorTop();

        /// <summary>
        /// Obtains the vertical position of the console cursor. Use this method
        /// instead of <see cref="System.Console.CursorTop" /> to avoid triggering
        /// pending calls to <see cref="IConsoleOperations.ReadKeyAsync(CancellationToken)" />
        /// on Unix platforms.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> to observe.</param>
        /// <returns>The vertical position of the console cursor.</returns>
        int GetCursorTop(CancellationToken cancellationToken);

        /// <summary>
        /// Obtains the vertical position of the console cursor. Use this method
        /// instead of <see cref="System.Console.CursorTop" /> to avoid triggering
        /// pending calls to <see cref="IConsoleOperations.ReadKeyAsync(CancellationToken)" />
        /// on Unix platforms.
        /// </summary>
        /// <returns>
        /// A <see cref="Task{T}" /> representing the asynchronous operation. The
        /// <see cref="Task{T}.Result" /> property will return the vertical position
        /// of the console cursor.
        /// </returns>
        Task<int> GetCursorTopAsync();

        /// <summary>
        /// Obtains the vertical position of the console cursor. Use this method
        /// instead of <see cref="System.Console.CursorTop" /> to avoid triggering
        /// pending calls to <see cref="IConsoleOperations.ReadKeyAsync(CancellationToken)" />
        /// on Unix platforms.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> to observe.</param>
        /// <returns>
        /// A <see cref="Task{T}" /> representing the asynchronous operation. The
        /// <see cref="Task{T}.Result" /> property will return the vertical position
        /// of the console cursor.
        /// </returns>
        Task<int> GetCursorTopAsync(CancellationToken cancellationToken);

        int GetWindowLeft();
        Task<int> GetWindowLeftAsync();
        int GetWindowLeft(CancellationToken cancellationToken);
        Task<int> GetWindowLeftAsync(CancellationToken cancellationToken);

        int GetWindowTop();
        Task<int> GetWindowTopAsync();
        int GetWindowTop(CancellationToken cancellationToken);
        Task<int> GetWindowTopAsync(CancellationToken cancellationToken);

        int GetWindowWidth();
        Task<int> GetWindowWidthAsync();
        int GetWindowWidth(CancellationToken cancellationToken);
        Task<int> GetWindowWidthAsync(CancellationToken cancellationToken);
    }
}
