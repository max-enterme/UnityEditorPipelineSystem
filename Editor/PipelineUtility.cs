using System;
using UnityEditor;
using UnityEditorPipelineSystem.Core;
using UnityEngine;

namespace UnityEditorPipelineSystem.Editor
{
    public static class PipelineUtility
    {
        public static async void RunAsync()
        {
            var name = "Pipeline";
            IContextContainer contextContainer = new ContextContainer();
            ITask[] tasks = Array.Empty<ITask>();

            using var pipeline = new Pipeline(name, contextContainer, tasks);
            pipeline.PipelineLoggerFactory = UnityPipelineLogger.GetDefaultPipelineLoggerFactory(pipeline);
            await pipeline.RunAsync();

            if (Application.isBatchMode)
            {
                EditorApplication.Exit(0);
            }
        }
    }
}