using System;
using UnityEditorPipelineSystem.Editor.Contexts.CommandLineArgumentConversion.ValueConverters;
using UnityEngine;

namespace UnityEditorPipelineSystem.Editor.Contexts.CommandLineArgumentConversion
{
    [Serializable]
    public class CommandLineArgumentProperty
    {
        [field: SerializeField] public string OptionName { get; set; }
        [field: SerializeField] public string BindingField { get; set; }
        [field: SerializeField] public bool Required { get; set; }

        [SerializeReference] private IValueConverter converter;

        public void SetConverter(IValueConverter valueConverter)
        {
            converter = valueConverter;
        }
    }
}