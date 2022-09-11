using System.Collections.Generic;
using UnityEditorPipelineSystem.Core;
using UnityEngine;

namespace UnityEditorPipelineSystem.Editor.Contexts
{
    public abstract class ContextProvider : ScriptableObject
    {
        public virtual IReadOnlyCollection<(string name, IContext context)> GetContexts() => new[] { GetContext() };
        public abstract (string name, IContext context) GetContext();
    }
}