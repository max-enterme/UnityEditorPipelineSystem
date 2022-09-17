using UnityEngine;

namespace UnityEditorPipelineSystem.Editor.Contexts.CommandLineArgumentConversion.ValueConverters
{
    public class Int32ValueConverter : IValueConverter
    {
        [SerializeField] private int defaultValue;

        public object DefaultValue => defaultValue;

        public object Convert(string value) => int.Parse(value);
    }
}