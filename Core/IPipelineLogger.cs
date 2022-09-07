using System;
using System.Threading.Tasks;

namespace UnityEditorPipelineSystem.Core
{
    public interface IPipelineLogger : IContext
    {
        public Task LogProgressAsync(string message);
        public Task LogAsync(string message);
        public Task LogWarningAsync(string message);
        public Task LogErrorAsync(string message);
        public Task LogExceptionAsync(Exception exception);
    }
}