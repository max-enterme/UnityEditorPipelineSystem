using UnityEngine;

namespace UnityEditorPipelineSystem.Editor.Contexts.CommandLineArgumentConversion.ValueConverters
{
    public class UInt64ValueConverter : IValueConverter
    {
        [SerializeField] private ulong defaultValue;

        public object DefaultValue => defaultValue;

        public object Convert(string value) => ulong.Parse(value);
    }
}