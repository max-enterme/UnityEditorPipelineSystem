using System.Collections.Generic;
using UnityEditorPipelineSystem.Core;
using UnityEngine;

namespace UnityEditorPipelineSystem.Editor.Contexts.CommandLineArgumentConversion
{
    public abstract class CommandLineArgumentConverter : ScriptableObject
    {
        public virtual IReadOnlyCollection<(string name, IContext context)> ToContexts(ICommandLineArgumentContainer container) => new[] { ToContext(container) };
        public abstract (string name, IContext context) ToContext(ICommandLineArgumentContainer container);
    }
}