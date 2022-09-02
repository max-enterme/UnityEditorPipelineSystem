using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditorPipelineSystem.Injector;

namespace UnityEditorPipelineSystem
{
    public class Pipeline
    {
        public static async Task RunAsync(string pipelineName, IContextContainer contextContainer, IEnumerable<ITask> tasks)
        {
            PipelineLogger pipelineLogger = default;
            if (!contextContainer.ContainsContext<IPipelineLogger>())
            {
                pipelineLogger = new PipelineLogger(pipelineName);
                contextContainer.SetContext<IPipelineLogger>(pipelineLogger);
            }

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

            if (pipelineLogger != null)
            {
                await pipelineLogger.DisposeAsync().ConfigureAwait(false);
            }
        }

        private static async Task RunRecursiveAsync(IContextContainer contextContainer, ITaskCollection taskCollection)
        {
            var logger = contextContainer.GetContext<IPipelineLogger>();

            if (!taskCollection.When(contextContainer))
            {
                await logger.LogProgressAsync($"{DateTime.Now} [{taskCollection.Name}] Skip TaskCollection.");
                return;
            }

            {
                var start = DateTime.Now;
                await logger.LogProgressAsync($"{start} [{taskCollection.Name}] Start TaskCollection.");
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
                var finish = DateTime.Now;
                var elapsed = finish - start;
                await logger.LogProgressAsync($"{finish} [{taskCollection.Name}] Finished TaskCollection. ({elapsed.TotalSeconds}[s])");
            }

            {
                var start = DateTime.Now;
                await logger.LogProgressAsync($"{start} [{taskCollection.Name}] Start Post Process.");
                await taskCollection.PostAsync(contextContainer);
                var finish = DateTime.Now;
                var elapsed = finish - start;
                await logger.LogProgressAsync($"{finish} [{taskCollection.Name}] Finished Post Process. ({elapsed.TotalSeconds}[s])");
            }
        }

        private static async Task RunUnitAsync(IContextContainer contextContainer, ITask task)
        {
            if (task is ISyncableTask syncTask)
            {

                ContextInjector.Inject(contextContainer, task);

                var logger = contextContainer.GetContext<IPipelineLogger>();
                var start = DateTime.Now;
                await logger.LogProgressAsync($"{start} [{task.Name}] Start Task.");
                var ret = syncTask.Run(contextContainer);
                var finish = DateTime.Now;
                var elapsed = finish - start;
                await logger.LogProgressAsync($"{finish} [{task.Name}] Finish Task. ({elapsed.TotalSeconds}[sec])");

                if (IsError(ret))
                {
                    throw new Exception(ret.Message);
                }

                ContextInjector.Extract(contextContainer, task);
            }
            else if (task is IAsyncableTask asyncableTask)
            {
                ContextInjector.Inject(contextContainer, task);

                var logger = contextContainer.GetContext<IPipelineLogger>();
                var start = DateTime.Now;
                await logger.LogProgressAsync($"{start} [{task.Name}] Start Task. ");
                var ret = await asyncableTask.RunAsync(contextContainer);
                var finish = DateTime.Now;
                var elapsed = finish - start;
                await logger.LogProgressAsync($"{finish} [{task.Name}] Finish Task. ({elapsed.TotalSeconds}[sec])");

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