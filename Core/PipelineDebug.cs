using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnityEditorPipelineSystem.Core
{
    public class PipelineDebug
    {
        public static ILogger Logger { get; set; }

        public static async Task LogProgressAsync(string message) => await (Logger?.LogProgressAsync(GetActivePipelineName(), message) ?? Task.CompletedTask);
        public static async Task LogAsync(string message) => await (Logger?.LogAsync(GetActivePipelineName(), message) ?? Task.CompletedTask);
        public static async Task LogWarningAsync(string message) => await (Logger?.LogWarningAsync(GetActivePipelineName(), message) ?? Task.CompletedTask);
        public static async Task LogErrorAsync(string message) => await (Logger?.LogErrorAsync(GetActivePipelineName(), message) ?? Task.CompletedTask);
        public static async Task LogExceptionAsync(Exception exception) => await (Logger?.LogExceptionAsync(GetActivePipelineName(), exception) ?? Task.CompletedTask);

        public static void LogProgress(string message) => Logger?.LogProgress(GetActivePipelineName(), message);
        public static void Log(string message) => Logger?.Log(GetActivePipelineName(), message);
        public static void LogWarning(string message) => Logger?.LogWarning(GetActivePipelineName(), message);
        public static void LogError(string message) => Logger?.LogError(GetActivePipelineName(), message);
        public static void LogException(Exception exception) => Logger?.LogException(GetActivePipelineName(), exception);

        private static readonly Stack<Pipeline> pipelines = new Stack<Pipeline>();

        public static void PushActivePipeline(Pipeline pipeline)
        {
            pipelines.Push(pipeline);
        }

        public static void PopActivePipeline()
        {
            pipelines.Pop();
        }

        public static string GetActivePipelineName()
        {
            return pipelines.Any() ? pipelines.Peek().Name : default;
        }
    }
}