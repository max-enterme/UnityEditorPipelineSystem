using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace UnityEditorPipelineSystem.Core
{
    public interface ITask
    {
        public string Name { get; set; }
    }

    public interface ITaskCollection : ITask
    {
        public bool When(IContextContainer contextContainer) => true;
        public IEnumerable<ITask> EnumerateTasks();
        public Task PostAsync(IContextContainer contextContainer) => Task.CompletedTask;
    }

    public interface ISyncTask : ITask
    {
        public ITaskResult Run(IContextContainer contextContainer) => Run(contextContainer, CancellationToken.None);
        public ITaskResult Run(IContextContainer contextContainer, CancellationToken ct);
    }

    public interface IAsyncTask : ITask
    {
        public Task<ITaskResult> RunAsync(IContextContainer contextContainer) => RunAsync(contextContainer, CancellationToken.None);
        public Task<ITaskResult> RunAsync(IContextContainer contextContainer, CancellationToken ct);
    }
}