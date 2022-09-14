using System.Collections.Generic;
using UnityEditor;
using UnityEditorPipelineSystem.Core;
using UnityEngine;

namespace UnityEditorPipelineSystem.Editor.Contexts.CommandLineArgumentConversion
{
    [CreateAssetMenu(fileName = "GenericCommandLineArgumentConverter", menuName = "UnityEditorPipelineSystem/Contexts/CommandLineArgumentConverters/GenericCommandLineArgumentConverter")]
    public class GenericCommandLineArgumentConverter : CommandLineArgumentConverter
    {
        [SerializeField] private MonoScript script = default;
        [SerializeField] private string contextName = default;
        [SerializeField] private List<CommandLineArgumentProperty> properties = default;

        public override (string name, IContext context) ToContext(ICommandLineArgumentContainer container)
        {
            throw new System.NotImplementedException();
        }

        [ContextMenu("Build Properties")]
        public void BuildProperties()
        {
        }

        [ContextMenu("Clear Properties")]
        public void ClearProperties()
        {
        }

        private void OnValidate()
        {
            if (script != null)
            {
                return;
            }

            var type = script.GetClass();
            if (!typeof(IContext).IsAssignableFrom(type))
            {
                Debug.LogWarning($"The configured script ({script.name}) does not implement IContext.");
                script = default;
                return;
            }
        }
    }
}