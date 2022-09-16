using UnityEngine;

namespace UnityEditorPipelineSystem.Editor.Contexts.CommandLineArgumentConversion.ValueConverters
{
    public interface IValueConverter
    {
        public object DefaultValue { get; }
        public object Convert(string text);
    }

    public class BooleanValueConverter : IValueConverter
    {
        [SerializeField] private bool defaultValue;

        public object DefaultValue => defaultValue;

        public object Convert(string text) => bool.Parse(text);
    }

    public class ByteValueConverter : IValueConverter
    {
        [SerializeField] private byte defaultValue;

        public object DefaultValue => defaultValue;

        public object Convert(string text) => byte.Parse(text);
    }

    public class SByteValueConverter : IValueConverter
    {
        [SerializeField] private sbyte defaultValue;

        public object DefaultValue => defaultValue;

        public object Convert(string text) => sbyte.Parse(text);
    }

    public class CharValueConverter : IValueConverter
    {
        [SerializeField] private char defaultValue;

        public object DefaultValue => defaultValue;

        public object Convert(string text) => char.Parse(text);
    }

    public class DoubleValueConverter : IValueConverter
    {
        [SerializeField] private double defaultValue;

        public object DefaultValue => defaultValue;

        public object Convert(string text) => double.Parse(text);
    }

    public class SingleValueConverter : IValueConverter
    {
        [SerializeField] private float defaultValue;

        public object DefaultValue => defaultValue;

        public object Convert(string text) => float.Parse(text);
    }

    public class Int32ValueConverter : IValueConverter
    {
        [SerializeField] private int defaultValue;

        public object DefaultValue => defaultValue;

        public object Convert(string text) => int.Parse(text);
    }

    public class UInt32ValueConverter : IValueConverter
    {
        [SerializeField] private uint defaultValue;

        public object DefaultValue => defaultValue;

        public object Convert(string text) => uint.Parse(text);
    }

    public class Int64ValueConverter : IValueConverter
    {
        [SerializeField] private long defaultValue;

        public object DefaultValue => defaultValue;

        public object Convert(string text) => long.Parse(text);
    }

    public class UInt64ValueConverter : IValueConverter
    {
        [SerializeField] private ulong defaultValue;

        public object DefaultValue => defaultValue;

        public object Convert(string text) => ulong.Parse(text);
    }

    public class Int16ValueConverter : IValueConverter
    {
        [SerializeField] private short defaultValue;

        public object DefaultValue => defaultValue;

        public object Convert(string text) => short.Parse(text);
    }

    public class UInt16ValueConverter : IValueConverter
    {
        [SerializeField] private ushort defaultValue;

        public object DefaultValue => defaultValue;

        public object Convert(string text) => ushort.Parse(text);
    }

    public class StringValueConverter : IValueConverter
    {
        [SerializeField] private string defaultValue;

        public object DefaultValue => defaultValue;

        public object Convert(string text) => text;
    }

    //public class EnumValueConverter : IValueConverter
    //{
    //}

    //public class StructValueConverter : IValueConverter
    //{
    //}
}