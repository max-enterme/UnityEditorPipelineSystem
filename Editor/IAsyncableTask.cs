using System.Threading.Tasks;

namespace UnityEditorPipelineSystem
{
    public interface IAsyncableTask : ITask
    {
        public Task<ITaskResult> RunAsync(IContextContainer contextContainer);
    }
}