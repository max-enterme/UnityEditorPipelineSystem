using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityEditorPipelineSystem
{
    public interface IPipelineLogger : IContext
    {
        public Task LogProgressAsync(string message);
        public Task LogAsync(string message);
        public Task LogWarningAsync(string message);
        public Task LogErrorAsync(string message);
        public Task LogExceptionAsync(Exception exception);
    }

    public class PipelineLogger : IPipelineLogger, IDisposable, IAsyncDisposable
    {
        private enum LogType
        {
            Progress,
            Log,
            Warning,
            Error,
            Exception,
        }

        private readonly string pipelineName;

        private readonly Dictionary<string, StreamWriter> writerByFilePath = new Dictionary<string, StreamWriter>();
        private readonly Dictionary<LogType, StreamWriter> writerByLogType = new Dictionary<LogType, StreamWriter>();


        public PipelineLogger(string pipelineName, string logProgressFile = default, string logFilePath = default, string logWarningFilePath = default, string logErrorFilePath = default, string logExceptionFilePath = default)
        {
            this.pipelineName = pipelineName;

            GenerateWriter(logProgressFile, LogType.Progress);
            GenerateWriter(logFilePath, LogType.Log);
            GenerateWriter(logWarningFilePath, LogType.Warning);
            GenerateWriter(logErrorFilePath, LogType.Error);
            GenerateWriter(logExceptionFilePath, LogType.Exception);

        }

        private void GenerateWriter(string logFilePath, LogType logType)
        {
            if (!string.IsNullOrEmpty(logFilePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));
                if (!writerByFilePath.TryGetValue(logFilePath, out var writer))
                {
                    writer = new StreamWriter(logFilePath);
                    writerByFilePath[logFilePath] = writer;
                }

                writerByLogType[logType] = writer;
            }
            else
            {
                writerByLogType[logType] = null;
            }
        }

        public async Task LogProgressAsync(string message)
        {
            Debug.Log($"[{pipelineName}][Progress]{message}");

            if (writerByLogType[LogType.Progress] != null)
                await writerByLogType[LogType.Progress].WriteLineAsync(message).ConfigureAwait(false);
        }

        public async Task LogAsync(string message)
        {
            Debug.Log($"[{pipelineName}][Info]{message}");

            if (writerByLogType[LogType.Log] != null)
                await writerByLogType[LogType.Log].WriteLineAsync($"{message}\n{Environment.StackTrace}").ConfigureAwait(false);
        }

        public async Task LogWarningAsync(string message)
        {
            Debug.LogWarning($"[{pipelineName}][Warning]{message}");

            if (writerByLogType[LogType.Warning] != null)
                await writerByLogType[LogType.Warning].WriteLineAsync($"{message}\n{Environment.StackTrace}").ConfigureAwait(false);
        }

        public async Task LogErrorAsync(string message)
        {
            Debug.LogError($"[{pipelineName}][Error]{message}");

            if (writerByLogType[LogType.Error] != null)
                await writerByLogType[LogType.Error].WriteLineAsync($"{message}\n{Environment.StackTrace}").ConfigureAwait(false);
        }

        public async Task LogExceptionAsync(Exception exception)
        {
            Debug.LogException(exception);

            if (writerByLogType[LogType.Exception] != null)
                await writerByLogType[LogType.Exception].WriteLineAsync(exception.ToString()).ConfigureAwait(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            var cachedLoggers = writerByFilePath.Values.ToArray();
            writerByFilePath.Clear();
            writerByLogType.Clear();

            foreach (var logger in cachedLoggers)
            {
                logger.Dispose();
            }
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore().ConfigureAwait(false);

            Dispose(disposing: false);
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
            GC.SuppressFinalize(this);
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
        }

        protected virtual async ValueTask DisposeAsyncCore()
        {
            var cachedLoggers = writerByFilePath.Values.ToArray();
            writerByFilePath.Clear();
            writerByLogType.Clear();

            foreach (var logger in cachedLoggers)
            {
                await logger.DisposeAsync().ConfigureAwait(false);
            }
        }
    }
}