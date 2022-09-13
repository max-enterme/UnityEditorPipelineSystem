using System;
using UnityEditor;
using UnityEditorPipelineSystem.Core;
using UnityEngine;

namespace UnityEditorPipelineSystem.Editor.Tasks
{
    [CreateAssetMenu(fileName = "GenericTaskProvider", menuName = "UnityEditorPipelineSystem/Tasks/GenericTaskProvider")]
    public class GenericTaskProvider : TaskProvider
    {
        [SerializeField] private MonoScript script = default;
        [SerializeReference] private ITask instance = default;

        public override ITask GetTask()
        {
            instance.Name = name;
            return instance;
        }

        private void OnValidate()
        {
            if (script != null && instance != default && script.GetClass() == instance.GetType())
            {
                return;
            }

            if (script == default)
            {
                instance = default;
                return;
            }

            var type = script.GetClass();
            if (typeof(ITask).IsAssignableFrom(type))
            {
                instance = (ITask)Activator.CreateInstance(type);
            }
            else
            {
                Debug.LogWarning($"The configured script ({script.name}) does not implement ITask.");
                script = default;
                instance = default;
                return;
            }
        }
    }
}