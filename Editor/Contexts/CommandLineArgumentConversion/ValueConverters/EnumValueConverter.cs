using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityEditorPipelineSystem.Editor.Contexts.CommandLineArgumentConversion.ValueConverters
{
    [Serializable]
    public class EnumValueConverter : IValueConverter
    {
        [SerializeField] private string enumTypeName = default;
        [SerializeField] private string defaultValue = default;
        [SerializeField] private string delimiter = ",";

        private Type targetType = default;
        public Type GetTargetType()
        {
            return targetType ??= Type.GetType(enumTypeName);
        }

        public EnumValueConverter(Type targetType)
        {
            enumTypeName = targetType.AssemblyQualifiedName;
            this.targetType = targetType;
        }

        public object DefaultValue => Convert(defaultValue);

        public object Convert(string value)
        {
            var elements = value.Split(delimiter)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrEmpty(x));
            var result = default(int);
            foreach (var element in elements)
            {
                if (Enum.TryParse(GetTargetType(), element, out object ret))
                {
                    result |= (int)ret;
                }
            }

            return result;
        }
    }

    // FEATURE
    // ここの実装
    //[CustomPropertyDrawer(typeof(EnumValueConverter))]
    //public class EnumValueConverterPropertyDrawer : PropertyDrawer
    //{
    //}
}