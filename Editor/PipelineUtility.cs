using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditorPipelineSystem.Core;
using UnityEngine;

namespace UnityEditorPipelineSystem.Editor
{
    public static class PipelineUtility
    {
        public static async void RunAsyncFromBatchMode()
        {
            var cliOptions = CommandLineArgumentContainer.Create();

            var path = cliOptions.GetOptionValue("PipelineAssetPath");
            var pipelineAsset = AssetDatabase.LoadAssetAtPath<PipelineAsset>(path);
            await pipelineAsset.RunAsync();
        }

        public static Task RunAsync(string name, IContextContainer contextContainer, IReadOnlyCollection<ITask> tasks)
        {
            return RunAsync(name, contextContainer, tasks, UnityPipelineLogger.GetDefaultPipelineLoggerFactory);
        }

        public static async Task RunAsync(string name, IContextContainer contextContainer, IReadOnlyCollection<ITask> tasks, Func<Pipeline, Func<IPipelineLogger>> loggerFactory)
        {
            using var pipeline = new Pipeline(name, contextContainer, tasks);
            pipeline.PipelineLoggerFactory = loggerFactory(pipeline);
            await pipeline.RunAsync();

            if (Application.isBatchMode)
            {
                EditorApplication.Exit(0);
            }
        }
    }
}