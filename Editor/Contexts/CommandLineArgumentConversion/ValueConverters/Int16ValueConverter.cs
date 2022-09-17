using UnityEngine;

namespace UnityEditorPipelineSystem.Editor.Contexts.CommandLineArgumentConversion.ValueConverters
{
    public class Int16ValueConverter : IValueConverter
    {
        [SerializeField] private short defaultValue;

        public object DefaultValue => defaultValue;

        public object Convert(string value) => short.Parse(value);
    }
}