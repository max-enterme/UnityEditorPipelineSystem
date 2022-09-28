using System.Reflection;

namespace UnityEditorPipelineSystem.Editor.Contexts.CommandLineArgumentConversion.ValueConverters
{
    public interface IValueConverterFactory
    {
        public IValueConverter GetValueConverter(FieldInfo info);
    }
}