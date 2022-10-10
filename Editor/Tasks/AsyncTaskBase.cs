using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEditorPipelineSystem.Core;
using UnityEngine;

namespace UnityEditorPipelineSystem.Editor.Tasks
{
    public abstract class AsyncTaskBase : IAsyncTask
    {
        [SerializeField] protected string timeout;
        public TimeSpan Timeout => !string.IsNullOrEmpty(timeout) ? TimeSpan.Parse(timeout) : TimeSpan.Zero;

        public string Name { get; set; }

        public AsyncTaskBase()
        {
            Name = GetType().FullName;
        }

        public AsyncTaskBase(string name)
        {
            Name = name;
        }

        public abstract Task<ITaskResult> RunAsync(IContextContainer contextContainer, CancellationToken ct);
    }
}