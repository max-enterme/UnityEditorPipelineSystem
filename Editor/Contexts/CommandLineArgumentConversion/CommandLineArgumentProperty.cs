using System;
using UnityEditorPipelineSystem.Editor.Contexts.CommandLineArgumentConversion.ValueConverters;
using UnityEngine;

namespace UnityEditorPipelineSystem.Editor.Contexts.CommandLineArgumentConversion
{
    [Serializable]
    public class CommandLineArgumentProperty
    {
        [SerializeField] private string optionName;
        [SerializeField] private string bindingField;
        [SerializeField] private bool required;
        [SerializeReference] private IValueConverter converter;

        public void SetConverter(IValueConverter valueConverter)
        {
            converter = valueConverter;
        }
    }
}