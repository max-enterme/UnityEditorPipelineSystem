using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            var type = script != null ? script.GetClass() : null;
            if (type == null)
            {
                return;
            }

            // TODO
            // SerializeField のリスト取得
            // 各フィールドに対応する CommandLineArgumentProperty のリストを生成する
            var serializeFields = GetSerializeFields(type);
            foreach (var field in serializeFields)
            {
                Debug.Log($"{field.Name}:{field.FieldType}:{field.GetHashCode()}");
            }
        }

        private static IReadOnlyCollection<FieldInfo> GetSerializeFields(Type target)
        {
            var serializeFields = new HashSet<FieldInfo>();

            //for (var current = target; current != null; current = current.BaseType)
            var current = target;
            {
                serializeFields.UnionWith(current.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                    .Where(x => x.GetCustomAttribute<SerializeField>(true) != null));
            }

            return serializeFields;
        }

        [ContextMenu("Clear Properties")]
        public void ClearProperties()
        {
            properties.Clear();
        }

        private void OnValidate()
        {
            if (script == null)
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