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
    }
}