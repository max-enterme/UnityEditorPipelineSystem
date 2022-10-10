using System;
using System.Threading;
using UnityEditorPipelineSystem.Core;
using UnityEngine;

namespace UnityEditorPipelineSystem.Editor.Tasks
{
    public abstract class SyncTaskBase : ISyncTask
    {
        [SerializeField] protected string timeout;
        public TimeSpan Timeout => !string.IsNullOrEmpty(timeout) ? TimeSpan.Parse(timeout) : TimeSpan.Zero;

        public string Name { get; set; }

        public SyncTaskBase()
        {
            Name = GetType().FullName;
        }

        public SyncTaskBase(string name)
        {
            Name = name;
        }

        public abstract ITaskResult Run(IContextContainer contextContainer, CancellationToken ct);
    }
}