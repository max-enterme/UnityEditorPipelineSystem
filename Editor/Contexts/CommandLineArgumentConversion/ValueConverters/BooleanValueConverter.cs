using UnityEngine;

namespace UnityEditorPipelineSystem.Editor.Contexts.CommandLineArgumentConversion.ValueConverters
{
    public class BooleanValueConverter : IValueConverter
    {
        [SerializeField] private bool defaultValue;

        public object DefaultValue => defaultValue;

        public object Convert(string text) => bool.Parse(text);
    }
}