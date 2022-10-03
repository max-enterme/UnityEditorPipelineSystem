using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace UnityEditorPipelineSystem.Core
{
    public class Pipeline
    {
        public string Name { get; private set; }
        public bool IsBusy { get; private set; }

        private readonly IContextContainer contextContainer;
        private readonly IReadOnlyCollection<ITask> tasks;

        public Pipeline(string pipelineName, IContextContainer contextContainer, IReadOnlyCollection<ITask> tasks)
        {
            Name = pipelineName;
            this.contextContainer = contextContainer;
            this.tasks = tasks;
        }

        public async Task RunAsync(CancellationToken ct = default)
        {
            if (IsBusy)
                throw new InvalidOperationException($"already working: {Name}");

            PipelineDebug.PushActivePipeline(this);

            IsBusy = true;

            foreach (var task in tasks)
            {
                if (task is ITaskCollection taskCollection)
                {
                    await RunRecursiveAsync(taskCollection, ct);
                }
                else
                {
                    await RunUnitAsync(task, ct);
                }
            }

            IsBusy = false;

            PipelineDebug.PopActivePipeline();
        }

        private async Task RunRecursiveAsync(ITaskCollection taskCollection, CancellationToken ct)
        {
            if (!taskCollection.When(contextContainer))
            {
                await PipelineDebug.LogProgressAsync($"[{taskCollection.Name}] Skip TaskCollection.");
                return;
            }

            ct.ThrowIfCancellationRequested();

            {
                var start = DateTime.Now;
                await PipelineDebug.LogProgressAsync($"[{taskCollection.Name}] Start TaskCollection.");
                foreach (var task in taskCollection.EnumerateTasks())
                {
                    if (task is ITaskCollection nestedCollection)
                    {
                        await RunRecursiveAsync(nestedCollection, ct);
                    }
                    else
                    {
                        await RunUnitAsync(task, ct);
                    }
                }
                var finish = DateTime.Now;
                var elapsed = finish - start;
                await PipelineDebug.LogProgressAsync($"[{taskCollection.Name}] Finished TaskCollection. ({elapsed.TotalSeconds}[s])");
            }

            ct.ThrowIfCancellationRequested();

            {
                var start = DateTime.Now;
                await PipelineDebug.LogProgressAsync($"[{taskCollection.Name}] Start Post Process.");
                await taskCollection.PostAsync(contextContainer);
                var finish = DateTime.Now;
                var elapsed = finish - start;
                await PipelineDebug.LogProgressAsync($"[{taskCollection.Name}] Finished Post Process. ({elapsed.TotalSeconds}[s])");
            }
        }

        private async Task RunUnitAsync(ITask task, CancellationToken ct)
        {
            if (task is ISyncTask syncTask)
            {
                ContextInjector.Inject(contextContainer, task);

                var start = DateTime.Now;
                await PipelineDebug.LogProgressAsync($"[{task.Name}] Start Task.");
                var ret = syncTask.Run(contextContainer, ct);
                var finish = DateTime.Now;
                var elapsed = finish - start;
                await PipelineDebug.LogProgressAsync($"[{task.Name}] Finish Task. ({elapsed.TotalSeconds}[sec])");

                if (IsError(ret))
                {
                    throw new Exception(ret.Message);
                }

                ContextInjector.Extract(contextContainer, task);
            }
            else if (task is IAsyncTask asyncableTask)
            {
                ContextInjector.Inject(contextContainer, task);

                var start = DateTime.Now;
                await PipelineDebug.LogProgressAsync($"[{task.Name}] Start Task. ");
                var ret = await asyncableTask.RunAsync(contextContainer, ct);
                var finish = DateTime.Now;
                var elapsed = finish - start;
                await PipelineDebug.LogProgressAsync($"[{task.Name}] Finish Task. ({elapsed.TotalSeconds}[sec])");

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

            ct.ThrowIfCancellationRequested();
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