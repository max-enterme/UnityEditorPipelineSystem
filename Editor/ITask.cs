using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnityEditorPipelineSystem
{
    public interface ITask
    {
        public string Name => GetType().FullName;
    }

    public interface ITaskCollection : ITask
    {
        public bool When(IContextContainer contextContainer) => true;
        public IEnumerable<ITask> EnumerateTasks();
        public Task PostAsync(IContextContainer contextContainer) => default;
    }

    public interface ISyncableTask : ITask
    {
        public ITaskResult Run(IContextContainer contextContainer);
    }

    public interface IAsyncableTask : ITask
    {
        public Task<ITaskResult> RunAsync(IContextContainer contextContainer);
    }
}