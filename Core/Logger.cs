using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace UnityEditorPipelineSystem.Core
{
    public enum LogType
    {
        Progress,
        Log,
        Warning,
        Error,
        Exception,
    }

    public class Logger : ILogger
    {
        protected readonly Dictionary<string, StreamWriter> writerByFilePath = new Dictionary<string, StreamWriter>();
        protected readonly Dictionary<LogType, StreamWriter> writerByLogType = new Dictionary<LogType, StreamWriter>();

        public Logger(string logProgressFile = default, string logFilePath = default, string logWarningFilePath = default, string logErrorFilePath = default, string logExceptionFilePath = default)
        {
            GenerateWriter(logProgressFile, LogType.Progress);
            GenerateWriter(logFilePath, LogType.Log);
            GenerateWriter(logWarningFilePath, LogType.Warning);
            GenerateWriter(logErrorFilePath, LogType.Error);
            GenerateWriter(logExceptionFilePath, LogType.Exception);
        }

        protected virtual void GenerateWriter(string logFilePath, LogType logType)
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

        public virtual async Task LogProgressAsync(string pipelineName, string message)
        {
            if (writerByLogType[LogType.Progress] != null)
                await writerByLogType[LogType.Progress].WriteLineAsync(GetProgressMessageText(message)).ConfigureAwait(false);
        }

        public virtual async Task LogAsync(string pipelineName, string message)
        {
            if (writerByLogType[LogType.Log] != null)
                await writerByLogType[LogType.Log].WriteLineAsync(GetMessageText(message, Environment.StackTrace)).ConfigureAwait(false);
        }

        public virtual async Task LogWarningAsync(string pipelineName, string message)
        {
            if (writerByLogType[LogType.Warning] != null)
                await writerByLogType[LogType.Warning].WriteLineAsync(GetMessageText(message, Environment.StackTrace)).ConfigureAwait(false);
        }

        public virtual async Task LogErrorAsync(string pipelineName, string message)
        {
            if (writerByLogType[LogType.Error] != null)
                await writerByLogType[LogType.Error].WriteLineAsync(GetMessageText(message, Environment.StackTrace)).ConfigureAwait(false);
        }

        public virtual async Task LogExceptionAsync(string pipelineName, Exception exception)
        {
            if (writerByLogType[LogType.Exception] != null)
                await writerByLogType[LogType.Exception].WriteLineAsync(GetExceptionMessageText(exception)).ConfigureAwait(false);
        }

        public virtual void LogProgress(string pipelineName, string message) => writerByLogType[LogType.Progress]?.WriteLine(GetProgressMessageText(message));
        public virtual void Log(string pipelineName, string message) => writerByLogType[LogType.Log]?.WriteLine(GetMessageText(message, Environment.StackTrace));
        public virtual void LogWarning(string pipelineName, string message) => writerByLogType[LogType.Warning]?.WriteLine(GetMessageText(message, Environment.StackTrace));
        public virtual void LogError(string pipelineName, string message) => writerByLogType[LogType.Error]?.WriteLine(GetMessageText(message, Environment.StackTrace));
        public virtual void LogException(string pipelineName, Exception exception) => writerByLogType[LogType.Exception]?.WriteLine(GetExceptionMessageText(exception));

        protected virtual string GetProgressMessageText(string message)
            => $"[{DateTime.Now}]{message}";

        protected virtual string GetMessageText(string message, string stackTrace)
            => $"[{DateTime.Now}]\n{message}\n{Environment.StackTrace}\n";

        protected virtual string GetExceptionMessageText(Exception exception)
            => $"[{DateTime.Now}]\n{exception}\n";

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore().ConfigureAwait(false);

            Dispose(disposing: false);
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
            GC.SuppressFinalize(this);
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
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