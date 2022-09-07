namespace UnityEditorPipelineSystem.Core
{
    public interface ITaskResult
    {
        ReturnCode Code { get; }
        string Message { get; }
    }
}