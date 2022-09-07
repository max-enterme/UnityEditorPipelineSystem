using System;
using System.Reflection;

namespace UnityEditorPipelineSystem.Core
{
    [AttributeUsage(AttributeTargets.Field)]
    public class InjectContextAttribute : Attribute
    {
        public ContextUsage Usage { get; set; }
        public bool Optional { get; set; }
        public string Name { get; set; }
        public string BindingField { get; set; }

        public InjectContextAttribute(ContextUsage usage = ContextUsage.InOut, bool optional = false, string name = default, string bindingField = default)
        {
            Usage = usage;
            Optional = optional;
            Name = name;
            BindingField = bindingField;
        }
    }

    public enum ContextUsage
    {
        InOut,
        In,
        Out
    }

    public class ContextInjector
    {
        public static void Inject(IContextContainer contextContainer, object obj)
        {
            FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (FieldInfo field in fields)
            {
                object[] attrs = field.GetCustomAttributes(typeof(InjectContextAttribute), true);
                if (attrs.Length == 0)
                    continue;

                InjectContextAttribute attr = attrs[0] as InjectContextAttribute;
                if (attr == null || attr.Usage == ContextUsage.Out)
                    continue;

                var contextName = attr.Name;
                if (!string.IsNullOrEmpty(attr.BindingField))
                {
                    var bidingField = obj.GetType().GetField(attr.BindingField, BindingFlags.Instance | BindingFlags.NonPublic);
                    if (bidingField == null)
                        throw new InvalidOperationException($"No field found for BidingField. field: {field.Name}, binding: {attr.BindingField}");
                    contextName = bidingField.GetValue(obj) as string;
                }

                object injectionObject;
                if (field.FieldType == typeof(IContextContainer))
                    injectionObject = contextContainer;
                else if (!attr.Optional)
                    injectionObject = contextContainer.GetContext(field.FieldType, contextName);
                else
                {
                    IContext context;
                    contextContainer.TryGetContext(field.FieldType, contextName, out context);
                    injectionObject = context;
                }

                field.SetValue(obj, injectionObject);
            }
        }

        public static void Extract(IContextContainer contextContainer, object obj)
        {
            FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (FieldInfo field in fields)
            {
                object[] attrs = field.GetCustomAttributes(typeof(InjectContextAttribute), true);
                if (attrs.Length == 0)
                    continue;

                InjectContextAttribute attr = attrs[0] as InjectContextAttribute;
                if (attr == null || attr.Usage == ContextUsage.In)
                    continue;

                if (field.FieldType == typeof(IContext))
                    throw new InvalidOperationException("IBuildContext can only be used with the ContextUsage.In option.");

                IContext copntext = field.GetValue(obj) as IContext;
                if (!attr.Optional)
                    contextContainer.SetContext(field.FieldType, copntext);
                else if (copntext != null)
                    contextContainer.SetContext(field.FieldType, copntext);
            }
        }
    }
}