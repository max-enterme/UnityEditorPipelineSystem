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
            => await LogAsyncInternal(pipelineName, LogType.Progress, GetProgressMessageText(message));

        public virtual async Task LogAsync(string pipelineName, string message)
            => await LogAsyncInternal(pipelineName, LogType.Log, GetMessageText(message, Environment.StackTrace));

        public virtual async Task LogWarningAsync(string pipelineName, string message)
            => await LogAsyncInternal(pipelineName, LogType.Warning, GetMessageText(message, Environment.StackTrace));

        public virtual async Task LogErrorAsync(string pipelineName, string message)
            => await LogAsyncInternal(pipelineName, LogType.Error, GetMessageText(message, Environment.StackTrace));

        public virtual async Task LogExceptionAsync(string pipelineName, Exception exception)
            => await LogAsyncInternal(pipelineName, LogType.Exception, GetExceptionMessageText(exception));

        protected virtual async Task LogAsyncInternal(string pipelineName, LogType logType, string message)
        {
            if (writerByLogType[logType] != null)
            {
                await writerByLogType[logType].WriteLineAsync(message);
                await writerByLogType[logType].FlushAsync();
            }
        }

        public virtual void LogProgress(string pipelineName, string message) => LogInternal(pipelineName, LogType.Progress, GetProgressMessageText(message));
        public virtual void Log(string pipelineName, string message) => LogInternal(pipelineName, LogType.Log, GetMessageText(message, Environment.StackTrace));
        public virtual void LogWarning(string pipelineName, string message) => LogInternal(pipelineName, LogType.Warning, GetMessageText(message, Environment.StackTrace));
        public virtual void LogError(string pipelineName, string message) => LogInternal(pipelineName, LogType.Error, GetMessageText(message, Environment.StackTrace));
        public virtual void LogException(string pipelineName, Exception exception) => LogInternal(pipelineName, LogType.Exception, GetExceptionMessageText(exception));

        protected virtual void LogInternal(string pipeline, LogType logType, string message)
        {
            if (writerByLogType[logType] != null)
            {
                writerByLogType[logType].WriteLine(message);
                writerByLogType[logType].Flush();
            }
        }

        protected virtual string GetProgressMessageText(string message)
            => $"[{DateTime.Now}]{message}";

        protected virtual string GetMessageText(string message, string stackTrace)
            => $"[{DateTime.Now}]\n{message}\n{stackTrace}\n";

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