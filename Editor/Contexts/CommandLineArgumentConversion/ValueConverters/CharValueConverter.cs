using UnityEngine;

namespace UnityEditorPipelineSystem.Editor.Contexts.CommandLineArgumentConversion.ValueConverters
{
    public class CharValueConverter : IValueConverter
    {
        [SerializeField] private char defaultValue;

        public object DefaultValue => defaultValue;

        public object Convert(string value) => char.Parse(value);
    }
}