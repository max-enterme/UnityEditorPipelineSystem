using UnityEngine;

namespace UnityEditorPipelineSystem.Editor.Contexts.CommandLineArgumentConversion.ValueConverters
{
    public class SByteValueConverter : IValueConverter
    {
        [SerializeField] private sbyte defaultValue;

        public object DefaultValue => defaultValue;

        public object Convert(string value) => sbyte.Parse(value);
    }
}