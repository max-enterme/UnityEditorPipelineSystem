using System;
using System.Collections.Generic;

namespace UnityEditorPipelineSystem
{
    public class ContextContainer : IContextContainer
    {
        private readonly Dictionary<(string type, string name), IContext> contexts = new Dictionary<(string type, string name), IContext>();

        public bool ContainsContext<T>(string name = null) where T : IContext
        {
            throw new NotImplementedException();
        }

        public bool ContainsContext<T>(Type type, string name = null) where T : IContext
        {
            throw new NotImplementedException();
        }

        public T GetContext<T>(string name = null) where T : IContext
        {
            return (T)contexts[(typeof(T).FullName, name)];
        }

        public IContext GetContext(Type type, string name = null)
        {
            return contexts[(type.FullName, name)];
        }

        public void SetContext<T>(T context, string name = null) where T : IContext
        {
            contexts[(typeof(T).FullName, name)] = context;
        }

        public void SetContext(Type type, IContext context, string name = null)
        {
            throw new NotImplementedException();
        }

        public void SetContext(IContext context, string name = null)
        {
            throw new NotImplementedException();
        }

        public bool TryGetContext<T>(out T context) where T : IContext
        {
            throw new NotImplementedException();
        }

        public bool TryGetContext<T>(string name, out T context) where T : IContext
        {
            throw new NotImplementedException();
        }

        public bool TryGetContext(Type type, out IContext context)
        {
            throw new NotImplementedException();
        }

        public bool TryGetContext(Type type, string name, out IContext context)
        {
            throw new NotImplementedException();
        }
    }
}