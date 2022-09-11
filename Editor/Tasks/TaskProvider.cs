using UnityEditorPipelineSystem.Core;
using UnityEngine;

namespace UnityEditorPipelineSystem.Editor.Tasks
{
    public abstract class TaskProvider : ScriptableObject
    {
        public abstract ITask GetTask();
    }
}