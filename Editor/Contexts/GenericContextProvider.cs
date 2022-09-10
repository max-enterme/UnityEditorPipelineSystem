using System;
using UnityEditor;
using UnityEditorPipelineSystem.Core;
using UnityEngine;

namespace UnityEditorPipelineSystem.Editor.Contexts
{
    [CreateAssetMenu(fileName = "GenericContextProvider", menuName = "UnityEditorPipelineSystem/Contexts/GenericContextProvider")]
    public class GenericContextProvider : ScriptableObject
    {
        [SerializeField] MonoScript script = default;
        [SerializeReference] IContext instance = default;

        public IContext GetContext()
        {
            return instance;
        }

        private void OnValidate()
        {
            if (script == default)
            {
                instance = default;
                return;
            }

            var type = script.GetClass();
            if (typeof(IContext).IsAssignableFrom(type))
            {
                instance = (IContext)Activator.CreateInstance(type);
            }
            else
            {
                Debug.LogWarning($"The configured script ({script.name}) does not implement IContext.");
                script = default;
                instance = default;
                return;
            }
        }
    }
}