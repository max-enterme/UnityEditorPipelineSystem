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

            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct);

            if (taskCollection.Timeout > TimeSpan.Zero)
            {
                linkedCts.CancelAfter(taskCollection.Timeout);
            }

            linkedCts.Token.ThrowIfCancellationRequested();

            {
                var start = DateTime.Now;
                await PipelineDebug.LogProgressAsync($"[{taskCollection.Name}] Start TaskCollection.");
                foreach (var task in taskCollection.EnumerateTasks())
                {
                    if (task is ITaskCollection nestedCollection)
                    {
                        await RunRecursiveAsync(nestedCollection, linkedCts.Token);
                    }
                    else
                    {
                        await RunUnitAsync(task, linkedCts.Token);
                    }
                }
                var finish = DateTime.Now;
                var elapsed = finish - start;
                await PipelineDebug.LogProgressAsync($"[{taskCollection.Name}] Finished TaskCollection. ({elapsed.TotalSeconds}[s])");
            }

            linkedCts.Token.ThrowIfCancellationRequested();

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

                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
                if (task.Timeout > TimeSpan.Zero)
                {
                    linkedCts.CancelAfter(task.Timeout);
                }

                var ret = syncTask.Run(contextContainer, linkedCts.Token);

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

                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
                if (task.Timeout > TimeSpan.Zero)
                {
                    linkedCts.CancelAfter(task.Timeout);
                }

                var ret = await asyncableTask.RunAsync(contextContainer, linkedCts.Token);

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