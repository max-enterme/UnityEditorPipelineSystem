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
            var success = true;
            try
            {
                var cliOptions = CommandLineArgumentContainer.Create();
                using var logger = CreateLogger(cliOptions);
                PipelineDebug.Logger = logger;

                var path = cliOptions.GetOptionValue("PipelineAssetPath");
                var pipelineAsset = AssetDatabase.LoadAssetAtPath<PipelineAsset>(path);
                await pipelineAsset.RunAsync();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                success = false;
            }
            finally
            {
                PipelineDebug.Logger = default;

                if (Application.isBatchMode)
                {
                    EditorApplication.Exit(success ? 0 : 1);
                }
            }
        }

        public static async Task RunAsync(string name, IContextContainer contextContainer, IReadOnlyCollection<ITask> tasks)
        {
            using var pipeline = new Pipeline(name, contextContainer, tasks);
            await pipeline.RunAsync();
        }

        private static UnityPipelineLogger CreateLogger(CommandLineArgumentContainer cliOptions)
        {
            var logProgress = cliOptions.GetOrDefaultOptionValue("logProgress");
            var logVerbose = cliOptions.GetOrDefaultOptionValue("logVerbose");
            var logWarning = cliOptions.GetOrDefaultOptionValue("logWarning");
            var logError = cliOptions.GetOrDefaultOptionValue("logError");
            return new UnityPipelineLogger(logProgress, logVerbose, logWarning, logError, logError);
        }
    }
}