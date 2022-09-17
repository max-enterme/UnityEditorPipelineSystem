using UnityEngine;

namespace UnityEditorPipelineSystem.Editor.Contexts.CommandLineArgumentConversion.ValueConverters
{
    public class DoubleValueConverter : IValueConverter
    {
        [SerializeField] private double defaultValue;

        public object DefaultValue => defaultValue;

        public object Convert(string value) => double.Parse(value);
    }
}