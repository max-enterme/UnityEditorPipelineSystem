using System;
using UnityEditor;
using UnityEditorPipelineSystem.Core;
using UnityEngine;

namespace UnityEditorPipelineSystem.Editor.Contexts
{
    [CreateAssetMenu(menuName = "UnityEditorPipelineSystem/Contexts/GenericContextProvider")]
    public class GenericContextProvider : ContextProvider
    {
        [SerializeField] private MonoScript script = default;
        [SerializeField] private string contextName = default;
        [SerializeReference] private IContext instance = default;

        public override (string name, IContext context) GetContext() => (!string.IsNullOrEmpty(contextName) ? contextName : null, instance);

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