using System;
using System.Threading.Tasks;
using UnityEditorPipelineSystem.Core;
using UnityEngine;

namespace UnityEditorPipelineSystem.Editor
{
    public class UnityPipelineLogger : PipelineLogger
    {
        public static new Func<IPipelineLogger> GetDefaultPipelineLoggerFactory(Pipeline pipeline)
        {
            return () =>
            {
                var directory = $"Library/pkg.max-enterme.unityeditor-pipeline-system/Logs/{pipeline.Name}";
                return new UnityPipelineLogger(pipeline.Name, $"{directory}/progress.log", $"{directory}/verbose.log", $"{directory}/warning.log", $"{directory}/error.log", $"{directory}/error.log");
            };
        }

        public static Func<IPipelineLogger> GetBatchModeDefaultPipelineLoggerFactory(Pipeline pipeline)
        {
            return () =>
            {
                var cliOptions = CommandLineArgumentContainer.Create();
                var logProgress = cliOptions.GetOrDefaultOptionValue("logProgress");
                var logVerbose = cliOptions.GetOrDefaultOptionValue("logVerbose");
                var logWarning = cliOptions.GetOrDefaultOptionValue("logWarning");
                var logError = cliOptions.GetOrDefaultOptionValue("logError");
                return new UnityPipelineLogger(pipeline.Name, logProgress, logVerbose, logWarning, logError, logError);
            };
        }

        public UnityPipelineLogger(string pipelineName, string logProgressFile = default, string logFilePath = default, string logWarningFilePath = default, string logErrorFilePath = default, string logExceptionFilePath = default)
            : base(pipelineName, logProgressFile, logFilePath, logWarningFilePath, logErrorFilePath, logExceptionFilePath)
        {
            Application.logMessageReceivedThreaded += OnLogMessageReceivedThreaded;
        }

        public override async Task LogProgressAsync(string message)
        {
            Debug.Log($"[{pipelineName}][Progress]{message}");
            await base.LogProgressAsync(message);
        }

        public override async Task LogAsync(string message)
        {
            Debug.Log($"[{pipelineName}][Info]{message}");
            await base.LogAsync(message);
        }

        public override async Task LogWarningAsync(string message)
        {
            Debug.LogWarning($"[{pipelineName}][Warning]{message}");
            await base.LogWarningAsync(message);
        }

        public override async Task LogErrorAsync(string message)
        {
            Debug.LogError($"[{pipelineName}][Error]{message}");
            await base.LogErrorAsync(message);
        }

        public override async Task LogExceptionAsync(Exception exception)
        {
            Debug.LogException(exception);
            await base.LogExceptionAsync(exception);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Application.logMessageReceivedThreaded -= OnLogMessageReceivedThreaded;
            }

            base.Dispose(disposing);
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            Application.logMessageReceivedThreaded -= OnLogMessageReceivedThreaded;

            await base.DisposeAsyncCore();
        }

        private async void OnLogMessageReceivedThreaded(string condition, string stackTrace, UnityEngine.LogType type)
        {
            if (condition.StartsWith($"[{pipelineName}]"))
            {
                return;
            }

            switch (type)
            {
                case UnityEngine.LogType.Log:
                    await LogAsync(condition);
                    break;
                case UnityEngine.LogType.Warning:
                    await LogWarningAsync(condition);
                    break;
                case UnityEngine.LogType.Error:
                case UnityEngine.LogType.Assert:
                    await LogErrorAsync(condition);
                    break;
                case UnityEngine.LogType.Exception:
                    await LogExceptionAsync(new Exception(condition));
                    break;
            }
        }
    }
}