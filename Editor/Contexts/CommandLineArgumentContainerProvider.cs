using System.Collections.Generic;
using UnityEditorPipelineSystem.Core;
using UnityEditorPipelineSystem.Editor.Contexts.CommandLineArgumentConversion;
using UnityEngine;

namespace UnityEditorPipelineSystem.Editor.Contexts
{
    public class CommandLineArgumentContainerProvider : ContextProvider
    {
        [SerializeField] private List<CommandLineArgumentConverter> converters = default;

        public override IReadOnlyCollection<(string name, IContext context)> GetContexts()
        {
            var contexts = new List<(string name, IContext context)>();

            var argContainer = CommandLineArgumentContainer.Create();
            contexts.Add((default, argContainer));

            foreach (var converter in converters)
            {
                contexts.AddRange(converter.ToContexts(argContainer));
            }

            return contexts;
        }

        public override (string name, IContext context) GetContext()
        {
            return (default, CommandLineArgumentContainer.Create());
        }
    }
}