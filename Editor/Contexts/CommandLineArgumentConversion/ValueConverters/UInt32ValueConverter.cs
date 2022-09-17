using UnityEngine;

namespace UnityEditorPipelineSystem.Editor.Contexts.CommandLineArgumentConversion.ValueConverters
{
    public class UInt32ValueConverter : IValueConverter
    {
        [SerializeField] private uint defaultValue;

        public object DefaultValue => defaultValue;

        public object Convert(string value) => uint.Parse(value);
    }
}