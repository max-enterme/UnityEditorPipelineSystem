using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorPipelineSystem.Core;
using UnityEditorPipelineSystem.Editor.Contexts.CommandLineArgumentConversion.ValueConverters;
using UnityEngine;

namespace UnityEditorPipelineSystem.Editor.Contexts.CommandLineArgumentConversion
{
    [CreateAssetMenu(fileName = "GenericCommandLineArgumentConverter", menuName = "UnityEditorPipelineSystem/Contexts/CommandLineArgumentConverters/GenericCommandLineArgumentConverter")]
    public class GenericCommandLineArgumentConverter : CommandLineArgumentConverter
    {
        [SerializeField] private MonoScript script = default;
        [SerializeField] private string contextName = default;
        [SerializeField] private List<CommandLineArgumentProperty> properties = default;

        public override (string name, IContext context) ToContext(ICommandLineArgumentContainer container)
        {
            var context = (IContext)Activator.CreateInstance(script.GetClass());

            foreach (var property in properties)
            {
                if (property.Converter == null)
                {
                    continue;
                }

                var value = container.TryGetOptionValue(property.OptionName, out var text)
                    ? property.Converter.Convert(text)
                    : property.Converter.DefaultValue;

                var tmp = property.BindingField.Split(':');
                var className = tmp[0];
                var fieldName = tmp[1];
                var targetType = Type.GetType(className);
                var fieldInfo = targetType.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                fieldInfo.SetValue(context, value);
            }

            return (!string.IsNullOrEmpty(contextName) ? contextName : null, context);
        }

        [ContextMenu("Build Properties")]
        public void BuildProperties()
        {
            var type = script != null ? script.GetClass() : null;
            if (type == null)
            {
                return;
            }

            var serializeFields = GetSerializeFields(type);
            foreach (var field in serializeFields)
            {
                var property = new CommandLineArgumentProperty();

                var fieldName = field.Name;
                if (fieldName.StartsWith("<"))
                {
                    fieldName = fieldName.Substring(1, fieldName.IndexOf(">") - 1);
                }

                if (!string.IsNullOrEmpty(contextName))
                {
                    property.OptionName = $"{contextName}.{fieldName}";
                }
                else
                {
                    property.OptionName = $"{type.Name}.{fieldName}";
                }

                if (properties.Any(x => x.OptionName == property.OptionName))
                {
                    continue;
                }

                property.BindingField = $"{field.DeclaringType.AssemblyQualifiedName}:{field.Name}";
                property.Converter = GetValueConverter(field);

                properties.Add(property);
            }
        }

        private static IReadOnlyCollection<FieldInfo> GetSerializeFields(Type target)
        {
            var serializeFields = new Dictionary<string, FieldInfo>();

            for (var current = target; current != null; current = current.BaseType)
            {
                var fields = current.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                    .Where(x => x.GetCustomAttribute<SerializeField>(true) != null);

                foreach (var field in fields)
                {
                    serializeFields[field.Name] = field;
                }
            }

            return serializeFields.Values.ToArray();
        }

        private IValueConverter GetValueConverter(FieldInfo info)
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

        [ContextMenu("Clear Properties")]
        public void ClearProperties()
        {
            properties.Clear();
        }

        private void OnValidate()
        {
            if (script == null)
            {
                return;
            }

            var type = script.GetClass();
            if (!typeof(IContext).IsAssignableFrom(type))
            {
                Debug.LogWarning($"The configured script ({script.name}) does not implement IContext.");
                script = default;
                return;
            }
        }
    }
}