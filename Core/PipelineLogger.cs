using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace UnityEditorPipelineSystem.Core
{
    public class PipelineLogger : IPipelineLogger, IDisposable, IAsyncDisposable
    {
        protected enum LogType
        {
            Progress,
            Log,
            Warning,
            Error,
            Exception,
        }

        public static Func<IPipelineLogger> GetDefaultPipelineLoggerFactory(Pipeline pipeline)
        {
            return () =>
            {
                var directory = $"Library/pkg.max-enterme.unityeditor-pipeline-system/Logs/{pipeline.Name}";
                return new PipelineLogger(pipeline.Name, $"{directory}/progress.log", $"{directory}/verbose.log", $"{directory}/warning.log", $"{directory}/error.log", $"{directory}/error.log");
            };
        }

        public static Func<IPipelineLogger> GetNonePipelineLoggerFactory(Pipeline pipeline)
        {
            return () =>
            {
                return new PipelineLogger(pipeline.Name);
            };
        }

        protected readonly string pipelineName;

        protected readonly Dictionary<string, StreamWriter> writerByFilePath = new Dictionary<string, StreamWriter>();
        protected readonly Dictionary<LogType, StreamWriter> writerByLogType = new Dictionary<LogType, StreamWriter>();

        public PipelineLogger(string pipelineName, string logProgressFile = default, string logFilePath = default, string logWarningFilePath = default, string logErrorFilePath = default, string logExceptionFilePath = default)
        {
            this.pipelineName = pipelineName;

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

        public virtual async Task LogProgressAsync(string message)
        {
            if (writerByLogType[LogType.Progress] != null)
                await writerByLogType[LogType.Progress].WriteLineAsync(message).ConfigureAwait(false);
        }

        public virtual async Task LogAsync(string message)
        {
            if (writerByLogType[LogType.Log] != null)
                await writerByLogType[LogType.Log].WriteLineAsync($"{message}\n{Environment.StackTrace}").ConfigureAwait(false);
        }

        public virtual async Task LogWarningAsync(string message)
        {
            if (writerByLogType[LogType.Warning] != null)
                await writerByLogType[LogType.Warning].WriteLineAsync($"{message}\n{Environment.StackTrace}").ConfigureAwait(false);
        }

        public virtual async Task LogErrorAsync(string message)
        {
            if (writerByLogType[LogType.Error] != null)
                await writerByLogType[LogType.Error].WriteLineAsync($"{message}\n{Environment.StackTrace}").ConfigureAwait(false);
        }

        public virtual async Task LogExceptionAsync(Exception exception)
        {
            if (writerByLogType[LogType.Exception] != null)
                await writerByLogType[LogType.Exception].WriteLineAsync(exception.ToString()).ConfigureAwait(false);
        }

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