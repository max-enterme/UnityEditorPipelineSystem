namespace UnityEditorPipelineSystem.Editor.Contexts.CommandLineArgumentConversion.ValueConverters
{
    public interface IValueConverter
    {
        public object DefaultValue { get; }
        public object Convert(string value);
    }

    //public class EnumValueConverter : IValueConverter
    //{
    //}

    //public class StructValueConverter : IValueConverter
    //{
    //}
}