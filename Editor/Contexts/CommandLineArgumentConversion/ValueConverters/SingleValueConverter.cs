using UnityEngine;

namespace UnityEditorPipelineSystem.Editor.Contexts.CommandLineArgumentConversion.ValueConverters
{
    public class SingleValueConverter : IValueConverter
    {
        [SerializeField] private float defaultValue;

        public object DefaultValue => defaultValue;

        public object Convert(string value) => float.Parse(value);
    }
}