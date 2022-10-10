using System;
using System.Collections.Generic;

namespace UnityEditorPipelineSystem.Core
{
    public class TaskCollection : ITaskCollection
    {
        public string Name { get; set; } = typeof(TaskCollection).FullName;
        public TimeSpan Timeout { get; set; } = TimeSpan.Zero;

        public readonly List<ITask> Tasks = new List<ITask>();
        public IEnumerable<ITask> EnumerateTasks() => Tasks;

        public TaskCollection() { }

        public TaskCollection(string name)
        {
            Name = name;
        }

        public TaskCollection(IEnumerable<ITask> tasks)
        {
            Tasks.AddRange(tasks);
        }

        public TaskCollection(string name, IEnumerable<ITask> tasks)
        {
            Name = name;
            Tasks.AddRange(tasks);
        }
    }
}