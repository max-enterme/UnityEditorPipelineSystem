using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditorPipelineSystem.Core;
using UnityEditorPipelineSystem.Editor.Contexts;
using UnityEditorPipelineSystem.Editor.Tasks;
using UnityEngine;

namespace UnityEditorPipelineSystem.Editor
{
    [CreateAssetMenu(fileName = "PipelineAsset", menuName = "UnityEditorPipelineSystem/PipelineAsset")]
    public class PipelineAsset : ScriptableObject
    {
        [SerializeField] private List<ContextProvider> contextProviders = default;
        [SerializeField] private List<TaskProvider> taskProviders = default;

        [ContextMenu("RunAsync")]
        private async void RunAsyncForContextMenu()
        {
            try
            {
                using var logger = CreateLogger(name);
                PipelineDebug.Logger = logger;
                await RunAsync();
            }
            catch
            {
                throw;
            }
            finally
            {
                PipelineDebug.Logger = default;
            }
        }

        public async Task RunAsync()
        {
            var contextContainer = new ContextContainer();

            foreach (var (name, context) in contextProviders.SelectMany(x => x.GetContexts()))
            {
                contextContainer.SetContext(context, name);
            }

            var tasks = taskProviders.Select(x => x.GetTask()).ToArray();
            await PipelineUtility.RunAsync(name, contextContainer, tasks);
        }

        private static Core.ILogger CreateLogger(string name)
        {
            var directory = $"Library/pkg.max-enterme.unityeditor-pipeline-system/Logs/{name}";
            return new UnityPipelineLogger($"{directory}/progress.log", $"{directory}/verbose.log", $"{directory}/warning.log", $"{directory}/error.log", $"{directory}/error.log");
        }
    }
}