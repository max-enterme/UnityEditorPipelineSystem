using UnityEngine;

namespace UnityEditorPipelineSystem.Editor.Contexts.CommandLineArgumentConversion.ValueConverters
{
    public class Int64ValueConverter : IValueConverter
    {
        [SerializeField] private long defaultValue;

        public object DefaultValue => defaultValue;

        public object Convert(string value) => long.Parse(value);
    }
}