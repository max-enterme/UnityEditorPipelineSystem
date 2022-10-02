using System.Collections.Generic;
using UnityEditorPipelineSystem.Core;
using UnityEngine;

namespace UnityEditorPipelineSystem.Editor.Tasks
{
    [CreateAssetMenu(menuName = "UnityEditorPipelineSystem/Tasks/TaskCollectionProvider")]
    public class TaskCollectionProvider : TaskProvider
    {
        [SerializeField] private List<TaskProvider> taskProviders = default;

        public override ITask GetTask()
        {
            var tasks = new TaskCollection(name);
            foreach (var provider in taskProviders)
            {
                tasks.Tasks.Add(provider.GetTask());
            }
            return tasks;
        }
    }
}