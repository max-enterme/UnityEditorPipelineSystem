using System;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityEditorPipelineSystem.Editor
{
    public class UnityPipelineLogger : Core.Logger, ILogHandler
    {
        private static ILogHandler defaultUnityLogHandler;
        private static ILogHandler GetDefaultUnityLogHandler()
        {
            return defaultUnityLogHandler ?? (defaultUnityLogHandler = Debug.unityLogger.logHandler);
        }

        private static void StoreDefaultUnityLogHandler()
        {
            _ = GetDefaultUnityLogHandler();
        }

        public UnityPipelineLogger(string logProgressFile = default, string logFilePath = default, string logWarningFilePath = default, string logErrorFilePath = default, string logExceptionFilePath = default)
            : base(logProgressFile, logFilePath, logWarningFilePath, logErrorFilePath, logExceptionFilePath)
        {
            StoreDefaultUnityLogHandler();
            Debug.unityLogger.logHandler = this;
        }

        public override async Task LogProgressAsync(string pipelineName, string message)
        {
            LogFormatInternal(LogType.Log, $"[{pipelineName}][Progress]{message}");
            await base.LogProgressAsync(pipelineName, message);
        }

        public override async Task LogAsync(string pipelineName, string message)
        {
            LogFormatInternal(LogType.Log, $"[{pipelineName}][Info]{message}");
            await base.LogAsync(pipelineName, message);
        }

        public override async Task LogWarningAsync(string pipelineName, string message)
        {
            LogFormatInternal(LogType.Warning, $"[{pipelineName}][Warning]{message}");
            await base.LogWarningAsync(pipelineName, message);
        }

        public override async Task LogErrorAsync(string pipelineName, string message)
        {
            LogFormatInternal(LogType.Error, $"[{pipelineName}][Error]{message}");
            await base.LogErrorAsync(pipelineName, message);
        }

        public override async Task LogExceptionAsync(string pipelineName, Exception exception)
        {
            GetDefaultUnityLogHandler().LogException(exception, null);
            await base.LogExceptionAsync(pipelineName, exception);
        }

        public override void LogProgress(string pipelineName, string message)
        {
            LogFormatInternal(LogType.Log, $"[{pipelineName}][Progress]{message}");
            base.LogProgress(pipelineName, message);
        }

        public override void Log(string pipelineName, string message)
        {
            LogFormatInternal(LogType.Log, $"[{pipelineName}][Info]{message}");
            base.Log(pipelineName, message);
        }

        public override void LogWarning(string pipelineName, string message)
        {
            LogFormatInternal(LogType.Warning, $"[{pipelineName}][Warning]{message}");
            base.LogWarning(pipelineName, message);
        }

        public override void LogError(string pipelineName, string message)
        {
            LogFormatInternal(LogType.Error, $"[{pipelineName}][Error]{message}");
            base.LogError(pipelineName, message);
        }

        public override void LogException(string pipelineName, Exception exception)
        {
            GetDefaultUnityLogHandler().LogException(exception, null);
            base.LogException(pipelineName, exception);
        }

        private void LogFormatInternal(LogType logType, string message)
        {
            GetDefaultUnityLogHandler().LogFormat(logType, null, "{0}", message);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Debug.unityLogger.logHandler = GetDefaultUnityLogHandler();
            }

            base.Dispose(disposing);
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            Debug.unityLogger.logHandler = GetDefaultUnityLogHandler();

            await base.DisposeAsyncCore();
        }

        public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
        {
            GetDefaultUnityLogHandler().LogFormat(logType, context, format, args);
            switch (logType)
            {
                case LogType.Log:
                    base.Log("UnityEngine.Debug.Log", string.Format(format, args));
                    break;
                case LogType.Warning:
                    base.LogWarning("UnityEngine.Debug.LogWarning", string.Format(format, args));
                    break;
                case LogType.Error:
                case LogType.Assert:
                    base.LogError("UnityEngine.Debug.LogError", string.Format(format, args));
                    break;
                case LogType.Exception:
                    base.LogException("UnityEngine.Debug.LogException", new Exception(string.Format(format, args)));
                    break;
            }
        }

        public void LogException(Exception exception, UnityEngine.Object context)
        {
            GetDefaultUnityLogHandler().LogException(exception, context);
            base.LogException("UnityEngine.Debug.LogException", exception);
        }
    }
}