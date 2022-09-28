using System.Reflection;
using UnityEngine;

namespace UnityEditorPipelineSystem.Editor.Contexts.CommandLineArgumentConversion.ValueConverters
{
    public abstract class ValueConverterFactoryProvider : ScriptableObject, IValueConverterFactory
    {
        public abstract IValueConverter GetValueConverter(FieldInfo info);
    }
}