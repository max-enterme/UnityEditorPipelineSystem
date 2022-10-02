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
                var path = cliOptions.GetOptionValue("PipelineAssetPath");
                var pipelineAsset = AssetDatabase.LoadAssetAtPath<PipelineAsset>(path);

                using var logger = CreateLogger(pipelineAsset, cliOptions);
                PipelineDebug.Logger = logger;

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

        private static Core.ILogger CreateLogger(PipelineAsset pipelineAsset, CommandLineArgumentContainer cliOptions)
        {
            if (pipelineAsset.OverrideLoggerProvider != null)
            {
                return pipelineAsset.OverrideLoggerProvider.CreateLogger();
            }

            var logProgress = cliOptions.GetOrDefaultOptionValue("logProgress");
            var logVerbose = cliOptions.GetOrDefaultOptionValue("logVerbose");
            var logWarning = cliOptions.GetOrDefaultOptionValue("logWarning");
            var logError = cliOptions.GetOrDefaultOptionValue("logError");
            return new Logger(logProgress, logVerbose, logWarning, logError, logError);
        }
    }
}