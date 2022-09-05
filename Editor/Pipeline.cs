using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEditorPipelineSystem.Injector;

namespace UnityEditorPipelineSystem
{
    public class Pipeline : IDisposable, IAsyncDisposable
    {
        public static async Task RunAsync(string pipelineName, IContextContainer contextContainer, IReadOnlyCollection<ITask> tasks, CancellationToken ct = default)
        {
            using var pipeline = new Pipeline(pipelineName, contextContainer, tasks);
            await pipeline.RunAsync(ct);
        }

        public string Name { get; private set; }

        private readonly IContextContainer contextContainer;
        private readonly IReadOnlyCollection<ITask> tasks;

        public event Action<string> OnStartTask;
        public event Action<string> OnFinishedTask;

        public Pipeline(string pipelineName, IContextContainer contextContainer, IReadOnlyCollection<ITask> tasks)
        {
            Name = pipelineName;
            this.contextContainer = contextContainer;
            this.tasks = tasks;
        }

        public async Task RunAsync(CancellationToken ct = default)
        {
            PipelineLogger pipelineLogger = default;
            if (!contextContainer.ContainsContext<IPipelineLogger>())
            {
                pipelineLogger = new PipelineLogger(Name);
                contextContainer.SetContext<IPipelineLogger>(pipelineLogger);
            }

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

            if (pipelineLogger != null)
            {
                await pipelineLogger.DisposeAsync().ConfigureAwait(false);
            }
        }


        private async Task RunRecursiveAsync(ITaskCollection taskCollection, CancellationToken ct)
        {
            var logger = contextContainer.GetContext<IPipelineLogger>();

            if (!taskCollection.When(contextContainer))
            {
                await logger.LogProgressAsync($"{DateTime.Now} [{taskCollection.Name}] Skip TaskCollection.");
                return;
            }

            ct.ThrowIfCancellationRequested();

            {
                var start = DateTime.Now;
                await logger.LogProgressAsync($"{start} [{taskCollection.Name}] Start TaskCollection.");
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
                await logger.LogProgressAsync($"{finish} [{taskCollection.Name}] Finished TaskCollection. ({elapsed.TotalSeconds}[s])");
            }

            ct.ThrowIfCancellationRequested();

            {
                var start = DateTime.Now;
                await logger.LogProgressAsync($"{start} [{taskCollection.Name}] Start Post Process.");
                await taskCollection.PostAsync(contextContainer);
                var finish = DateTime.Now;
                var elapsed = finish - start;
                await logger.LogProgressAsync($"{finish} [{taskCollection.Name}] Finished Post Process. ({elapsed.TotalSeconds}[s])");
            }
        }

        private async Task RunUnitAsync(ITask task, CancellationToken ct)
        {
            if (task is ISyncTask syncTask)
            {

                ContextInjector.Inject(contextContainer, task);

                var logger = contextContainer.GetContext<IPipelineLogger>();
                var start = DateTime.Now;
                await logger.LogProgressAsync($"{start} [{task.Name}] Start Task.");

                OnStartTask?.Invoke(task.Name);

                var ret = syncTask.Run(contextContainer, ct);

                OnFinishedTask?.Invoke(task.Name);


                var finish = DateTime.Now;
                var elapsed = finish - start;
                await logger.LogProgressAsync($"{finish} [{task.Name}] Finish Task. ({elapsed.TotalSeconds}[sec])");

                if (IsError(ret))
                {
                    throw new Exception(ret.Message);
                }

                ContextInjector.Extract(contextContainer, task);
            }
            else if (task is IAsyncTask asyncableTask)
            {
                ContextInjector.Inject(contextContainer, task);

                var logger = contextContainer.GetContext<IPipelineLogger>();
                var start = DateTime.Now;
                await logger.LogProgressAsync($"{start} [{task.Name}] Start Task. ");

                OnStartTask?.Invoke(task.Name);

                var ret = await asyncableTask.RunAsync(contextContainer);

                OnFinishedTask?.Invoke(task.Name);

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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore().ConfigureAwait(false);

            Dispose(disposing: false);
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
            GC.SuppressFinalize(this);
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
        }

        protected virtual async ValueTask DisposeAsyncCore()
        {
        }
    }
}