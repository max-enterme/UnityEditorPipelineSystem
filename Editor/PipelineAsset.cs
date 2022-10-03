using System.Collections.Generic;
using System.Linq;
using UnityEditorPipelineSystem.Core;
using UnityEditorPipelineSystem.Editor.Contexts;
using UnityEditorPipelineSystem.Editor.Tasks;
using UnityEngine;

namespace UnityEditorPipelineSystem.Editor
{
    [CreateAssetMenu(menuName = "UnityEditorPipelineSystem/PipelineAsset")]
    public class PipelineAsset : ScriptableObject
    {
        [field: SerializeField]
        public LoggerProvider OverrideLoggerProvider { get; private set; }

        [SerializeField] private List<ContextProvider> contextProviders = default;
        [SerializeField] private List<TaskProvider> taskProviders = default;

        [ContextMenu("RunAsync")]
        public async void RunAsync()
        {
            var pipeline = InstantiatePipeline();
            using var logger = CreateLogger(OverrideLoggerProvider, pipeline);
            await PipelineUtility.RunAsync(pipeline, logger);
        }

        public Pipeline InstantiatePipeline()
        {
            var contextContainer = new ContextContainer();

            foreach (var (name, context) in contextProviders.SelectMany(x => x.GetContexts()))
            {
                contextContainer.SetContext(context, name);
            }

            var tasks = taskProviders.Select(x => x.GetTask()).ToArray();
            return PipelineUtility.InstantiatePipeline(name, contextContainer, tasks);
        }

        private static Core.ILogger CreateLogger(LoggerProvider loggerProvider, Pipeline pipeline)
        {
            if (loggerProvider != null)
            {
                return loggerProvider.CreateLogger(pipeline);
            }

            var directory = $"Library/pkg.max-enterme.unityeditor-pipeline-system/Logs/{pipeline.Name}";
            return new Logger($"{directory}/progress.log", $"{directory}/verbose.log", $"{directory}/warning.log", $"{directory}/error.log", $"{directory}/error.log");
        }
    }
}