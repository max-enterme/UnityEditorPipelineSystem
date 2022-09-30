using System;
using System.Threading.Tasks;

namespace UnityEditorPipelineSystem.Core
{
    public interface ILogger : IDisposable, IAsyncDisposable
    {
        public Task LogProgressAsync(string pipelineName, string message);
        public Task LogAsync(string pipelineName, string message);
        public Task LogWarningAsync(string pipelineName, string message);
        public Task LogErrorAsync(string pielineName, string message);
        public Task LogExceptionAsync(string pipelineName, Exception exception);

        public void LogProgress(string pipelineName, string message);
        public void Log(string pipelineName, string message);
        public void LogWarning(string pipelineName, string message);
        public void LogError(string pipelineName, string message);
        public void LogException(string pipelineName, Exception exception);
    }
}