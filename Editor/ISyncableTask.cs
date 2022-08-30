namespace UnityEditorPipelineSystem
{
    public interface ISyncableTask : ITask
    {
        public ITaskResult Run(IContextContainer contextContainer);
    }
}