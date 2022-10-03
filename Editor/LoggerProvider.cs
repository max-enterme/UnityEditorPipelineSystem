using UnityEditorPipelineSystem.Core;
using UnityEngine;

namespace UnityEditorPipelineSystem.Editor
{
    public abstract class LoggerProvider : ScriptableObject
    {
        public abstract Core.ILogger CreateLogger(Pipeline pipeline);
    }
}