//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.PowerShell.EditorServices.Utility;

namespace Microsoft.PowerShell.EditorServices.Console
{
    internal class WindowsConsoleOperations : IConsoleOperations
    {
        private ConsoleKeyInfo? _bufferedKey;

        private SemaphoreSlim _readKeyHandle = AsyncUtils.CreateSimpleLockingSemaphore();

        public int GetCursorLeft() => System.Console.CursorLeft;

        public int GetCursorLeft(CancellationToken cancellationToken) => System.Console.CursorLeft;

        public Task<int> GetCursorLeftAsync() => Task.FromResult(System.Console.CursorLeft);

        public Task<int> GetCursorLeftAsync(CancellationToken cancellationToken) => Task.FromResult(System.Console.CursorLeft);

        public int GetCursorTop() => System.Console.CursorTop;

        public int GetCursorTop(CancellationToken cancellationToken) => System.Console.CursorTop;

        public Task<int> GetCursorTopAsync() => Task.FromResult(System.Console.CursorTop);

        public Task<int> GetCursorTopAsync(CancellationToken cancellationToken) => Task.FromResult(System.Console.CursorTop);

        public int GetWindowLeft() => System.Console.WindowLeft;

        public Task<int> GetWindowLeftAsync() => Task.FromResult(System.Console.WindowLeft);

        public int GetWindowLeft(CancellationToken cancellationToken) => System.Console.WindowLeft;

        public Task<int> GetWindowLeftAsync(CancellationToken cancellationToken) => Task.FromResult(System.Console.WindowLeft);

        public int GetWindowTop() => System.Console.WindowTop;

        public Task<int> GetWindowTopAsync() => Task.FromResult(System.Console.WindowTop);

        public int GetWindowTop(CancellationToken cancellationToken) => System.Console.WindowTop;

        public Task<int> GetWindowTopAsync(CancellationToken cancellationToken) => Task.FromResult(System.Console.WindowTop);

        public int GetWindowWidth() => System.Console.WindowWidth;

        public Task<int> GetWindowWidthAsync() => Task.FromResult(System.Console.WindowWidth);

        public int GetWindowWidth(CancellationToken cancellationToken) => System.Console.WindowWidth;

        public Task<int> GetWindowWidthAsync(CancellationToken cancellationToken) => Task.FromResult(System.Console.WindowWidth);

        public async Task<ConsoleKeyInfo> ReadKeyAsync(CancellationToken cancellationToken)
        {
            await _readKeyHandle.WaitAsync(cancellationToken);
            try
            {
                return
                    _bufferedKey.HasValue
                        ? _bufferedKey.Value
                        : await Task.Factory.StartNew(
                            () => (_bufferedKey = System.Console.ReadKey(intercept: true)).Value);
            }
            finally
            {
                _readKeyHandle.Release();

                // Throw if we're cancelled so the buffered key isn't cleared.
                cancellationToken.ThrowIfCancellationRequested();
                _bufferedKey = null;
            }
        }

        public void SetCursorPosition(int left, int top) => System.Console.SetCursorPosition(left, top);

        public Task SetCursorPositionAsync(int left, int top)
        {
            System.Console.SetCursorPosition(left, top);
            return Task.CompletedTask;
        }

        public void SetCursorPosition(int left, int top, CancellationToken cancellationToken) =>
            System.Console.SetCursorPosition(left, top);

        public Task SetCursorPositionAsync(int left, int top, CancellationToken cancellationToken)
        {
            System.Console.SetCursorPosition(left, top);
            return Task.CompletedTask;
        }

        public void Write(char value) => System.Console.Write(value);

        public Task WriteAsync(char value)
        {
            System.Console.Write(value);
            return Task.CompletedTask;
        }

        public void Write(char value, CancellationToken cancellationToken) => System.Console.Write(value);

        public Task WriteAsync(char value, CancellationToken cancellationToken)
        {
            System.Console.Write(value);
            return Task.CompletedTask;
        }

        public void Write(string value) => System.Console.Write(value);

        public Task WriteAsync(string value)
        {
            System.Console.Write(value);
            return Task.CompletedTask;
        }

        public void Write(string value, CancellationToken cancellationToken) => System.Console.Write(value);

        public Task WriteAsync(string value, CancellationToken cancellationToken)
        {
            System.Console.Write(value);
            return Task.CompletedTask;
        }
    }
}
