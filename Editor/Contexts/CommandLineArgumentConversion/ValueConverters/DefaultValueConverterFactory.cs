using System;
using System.Reflection;

namespace UnityEditorPipelineSystem.Editor.Contexts.CommandLineArgumentConversion.ValueConverters
{
    public class DefaultValueConverterFactory : IValueConverterFactory
    {
        public IValueConverter GetValueConverter(FieldInfo info)
        {
            if (info.FieldType.IsEnum)
            {
                return new EnumValueConverter(info.FieldType);
            }

            if (StructValueConverter.IsStructType(info.FieldType))
            {
                return new StructValueConverter(info.FieldType);
            }

            switch (Type.GetTypeCode(info.FieldType))
            {
                case TypeCode.Boolean:
                    return new BooleanValueConverter();
                case TypeCode.Byte:
                    return new ByteValueConverter();
                case TypeCode.SByte:
                    return new SByteValueConverter();
                case TypeCode.Char:
                    return new CharValueConverter();
                case TypeCode.Double:
                    return new DoubleValueConverter();
                case TypeCode.Single:
                    return new SingleValueConverter();
                case TypeCode.Int32:
                    return new Int32ValueConverter();
                case TypeCode.UInt32:
                    return new UInt32ValueConverter();
                case TypeCode.Int64:
                    return new Int64ValueConverter();
                case TypeCode.UInt64:
                    return new UInt64ValueConverter();
                case TypeCode.Int16:
                    return new Int16ValueConverter();
                case TypeCode.UInt16:
                    return new UInt16ValueConverter();
                default:
                    return default;
            }
        }
    }
}