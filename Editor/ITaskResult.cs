namespace UnityEditorPipelineSystem
{
    public interface ITaskResult
    {
        ReturnCode Code { get; }
        string Message { get; }
    }
}