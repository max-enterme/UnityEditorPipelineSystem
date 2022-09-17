using UnityEngine;

namespace UnityEditorPipelineSystem.Editor.Contexts.CommandLineArgumentConversion.ValueConverters
{
    public class StringValueConverter : IValueConverter
    {
        [SerializeField] private string defaultValue;

        public object DefaultValue => defaultValue;

        public object Convert(string value) => value;
    }
}