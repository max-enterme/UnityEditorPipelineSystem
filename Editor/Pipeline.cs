using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditorPipelineSystem.Injector;
using UnityEngine;

namespace UnityEditorPipelineSystem
{
    public class Pipeline
    {
        public static async Task RunAsync(IContextContainer contextContainer, IEnumerable<ITask> tasks)
        {
            var pipelineLogger = new PipelineLogger("Test", "./progress.log", "./info.log", "./warning.log", "./error.log", "./error.log");
            contextContainer.SetContext<IPipelineLogger>(pipelineLogger);

            foreach (var task in tasks)
            {
                if (task is ITaskCollection taskCollection)
                {
                    await RunRecursiveAsync(contextContainer, taskCollection);
                }
                else
                {
                    await RunUnitAsync(contextContainer, task);
                }
            }

            await pipelineLogger.DisposeAsync().ConfigureAwait(false);
        }

        private static async Task RunRecursiveAsync(IContextContainer contextContainer, ITaskCollection taskCollection)
        {
            if (!taskCollection.When(contextContainer))
            {
                Debug.Log("Skip");
                return;
            }

            foreach (var task in taskCollection.EnumerateTasks())
            {
                if (task is ITaskCollection nestedCollection)
                {
                    await RunRecursiveAsync(contextContainer, nestedCollection);
                }
                else
                {
                    await RunUnitAsync(contextContainer, task);
                }
            }

            await taskCollection.PostAsync(contextContainer);
        }

        private static async Task RunUnitAsync(IContextContainer contextContainer, ITask task)
        {
            if (task is ISyncableTask syncTask)
            {
                ContextInjector.Inject(contextContainer, task);
                var ret = syncTask.Run(contextContainer);
                if (IsError(ret))
                {
                    throw new Exception(ret.Message);
                }
                ContextInjector.Extract(contextContainer, task);
            }
            else if (task is IAsyncableTask asyncableTask)
            {
                ContextInjector.Inject(contextContainer, task);
                var ret = await asyncableTask.RunAsync(contextContainer);
                if (IsError(ret))
                {
                    throw new Exception(ret.Message);
                }
                ContextInjector.Extract(contextContainer, task);
            }
            else
            {
                throw new InvalidOperationException($"Invalid task type: {task.GetType()}");
            }
        }

        private static bool IsError(ITaskResult taskResult)
        {
            switch (taskResult.Code)
            {
                case ReturnCode.Error:
                    return true;
            }

            return false;
        }
    }
}