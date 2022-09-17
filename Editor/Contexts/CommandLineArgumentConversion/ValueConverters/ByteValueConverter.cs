using UnityEngine;

namespace UnityEditorPipelineSystem.Editor.Contexts.CommandLineArgumentConversion.ValueConverters
{
    public class ByteValueConverter : IValueConverter
    {
        [SerializeField] private byte defaultValue;

        public object DefaultValue => defaultValue;

        public object Convert(string value) => byte.Parse(value);
    }
}