using UnityEngine;

namespace UnityEditorPipelineSystem.Editor.Contexts.CommandLineArgumentConversion.ValueConverters
{
    public class UInt16ValueConverter : IValueConverter
    {
        [SerializeField] private ushort defaultValue;

        public object DefaultValue => defaultValue;

        public object Convert(string value) => ushort.Parse(value);
    }
}