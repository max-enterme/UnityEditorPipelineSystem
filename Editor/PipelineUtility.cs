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
            static PipelineAsset LoadPipelineAsset(CommandLineArgumentContainer cliOptions)
            {
                cliOptions = CommandLineArgumentContainer.Create();
                var path = cliOptions.GetOptionValue("PipelineAssetPath");
                return AssetDatabase.LoadAssetAtPath<PipelineAsset>(path);
            }

            var cliOptions = CommandLineArgumentContainer.Create();
            PipelineAsset pipelineAsset = default;
            try
            {
                pipelineAsset = LoadPipelineAsset(cliOptions);
            }
            catch (Exception e)
            {
                Debug.LogException(e);

                if (Application.isBatchMode)
                {
                    EditorApplication.Exit(1);
                }
            }

            var pipeline = pipelineAsset.InstantiatePipeline();
            using var logger = CreateLogger(pipelineAsset, pipeline, cliOptions);
            await RunAsync(pipeline, logger);
        }

        public static async Task RunAsync(Pipeline pipeline, Core.ILogger logger)
        {
            var success = true;
            try
            {
                PipelineDebug.Logger = logger;
                await pipeline.RunAsync();
            }
            catch (Exception e)
            {
                if (PipelineDebug.Logger != null)
                {
                    PipelineDebug.LogException(e);
                }
                else
                {
                    Debug.LogException(e);
                }

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

        public static Pipeline InstantiatePipeline(string name, IContextContainer contextContainer, IReadOnlyCollection<ITask> tasks)
        {
            return new Pipeline(name, contextContainer, tasks);
        }

        private static Core.ILogger CreateLogger(PipelineAsset pipelineAsset, Pipeline pipeline, CommandLineArgumentContainer cliOptions)
        {
            if (pipelineAsset.OverrideLoggerProvider != null)
            {
                return pipelineAsset.OverrideLoggerProvider.CreateLogger(pipeline);
            }

            var logProgress = cliOptions.GetOrDefaultOptionValue("logProgress");
            var logVerbose = cliOptions.GetOrDefaultOptionValue("logVerbose");
            var logWarning = cliOptions.GetOrDefaultOptionValue("logWarning");
            var logError = cliOptions.GetOrDefaultOptionValue("logError");
            return new Logger(logProgress, logVerbose, logWarning, logError, logError);
        }
    }
}