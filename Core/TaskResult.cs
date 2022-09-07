namespace UnityEditorPipelineSystem.Core
{
    public class TaskResult : ITaskResult
    {
        public static readonly TaskResult Success = new TaskResult(ReturnCode.Success, string.Empty);

        public ReturnCode Code { get; set; }
        public string Message { get; set; }

        public TaskResult(ReturnCode returnCode, string message)
        {
            Code = returnCode;
            Message = message;
        }
    }
}