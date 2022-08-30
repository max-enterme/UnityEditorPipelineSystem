using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditorPipelineSystem.Injector;

namespace UnityEditorPipelineSystem
{
    public class Pipeline
    {
        public static async Task RunAsync(IContextContainer contextContainer, IEnumerable<ITask> tasks)
        {
            foreach (var task in tasks)
            {
                if (task is ISyncableTask syncTask)
                {
                    ContextInjector.Inject(contextContainer, task);
                    var ret = syncTask.Run(contextContainer);
                    ContextInjector.Extract(contextContainer, task);
                }
                else if (task is IAsyncableTask asyncableTask)
                {
                    ContextInjector.Inject(contextContainer, task);
                    var ret = await asyncableTask.RunAsync(contextContainer);
                    ContextInjector.Extract(contextContainer, task);
                }
            }
        }
    }
}