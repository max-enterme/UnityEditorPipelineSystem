using System;
using UnityEngine;

namespace UnityEditorPipelineSystem.Editor.Contexts.CommandLineArgumentConversion.ValueConverters
{
    public class StructValueConverter : IValueConverter
    {
        [SerializeField] private string structTypeName = default;
        [SerializeReference] private object defaultValue = default;

        private Type targetType = default;
        public Type GetTargetType()
        {
            return targetType ??= Type.GetType(structTypeName);
        }

        public StructValueConverter(Type targetType)
        {
            structTypeName = targetType.AssemblyQualifiedName;
            defaultValue = Activator.CreateInstance(GetTargetType());
        }

        public object DefaultValue => defaultValue;

        public object Convert(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return Activator.CreateInstance(GetTargetType());
            }

            return JsonUtility.FromJson(value, GetTargetType());
        }

        public static bool IsStructType(Type type)
        {
            return type.IsValueType &&
                !type.IsPrimitive &&
                !type.IsEnum;
        }
    }
}