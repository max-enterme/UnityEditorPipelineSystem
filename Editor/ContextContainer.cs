using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEditorPipelineSystem
{
    public class ContextContainer : IContextContainer
    {
        private readonly Dictionary<(Type type, string name), IContext> contexts = new Dictionary<(Type type, string name), IContext>();

        public bool ContainsContext<T>(string name = null) where T : IContext
        {
            return ContainsContext(typeof(T), name);
        }

        public bool ContainsContext(Type type, string name = null)
        {
            _ = type ?? throw new ArgumentNullException(nameof(type));
            return contexts.ContainsKey((type, name));
        }

        public T GetContext<T>(string name = null) where T : IContext
        {
            return (T)GetContext(typeof(T), name);
        }

        public IContext GetContext(Type type, string name = null)
        {
            _ = type ?? throw new ArgumentNullException(nameof(type));

            if (!contexts.ContainsKey((type, name)))
                throw new Exception($"Context Id ({type}, {name}) was not available within the ContextContainer");

            return contexts[(type, name)];
        }

        private IEnumerable<Type> WalkAssignableTypes(IContext contextObject)
        {
            var iCType = typeof(IContext);
            foreach (Type t in contextObject.GetType().GetInterfaces())
            {
                if (iCType.IsAssignableFrom(t) && t != iCType)
                    yield return t;
            }

            for (var current = contextObject.GetType(); current != null; current = current.BaseType)
                if (iCType.IsAssignableFrom(current) && current != iCType)
                    yield return current;
        }

        public void SetContext<T>(T context, string name = null) where T : IContext
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));

            var type = typeof(T);
            if (!type.IsInterface)
                throw new InvalidOperationException(string.Format("Passed in type '{0}' is not an interface.", type));
            if (!(context is T))
                throw new InvalidOperationException(string.Format("'{0}' is not of passed in type '{1}'.", context.GetType(), type));

            contexts[(type, name)] = context;
        }

        public void SetContext(Type type, IContext context, string name = null)
        {
            _ = type ?? throw new ArgumentNullException(nameof(type));
            _ = context ?? throw new ArgumentNullException(nameof(context));

            if (!type.IsInterface)
                throw new InvalidOperationException(string.Format("Passed in type '{0}' is not an interface.", type));
            if (!type.IsInstanceOfType(context))
                throw new InvalidOperationException(string.Format("'{0}' is not of passed in type '{1}'.", context.GetType(), type));

            contexts[(type, name)] = context;
        }

        public void SetContext(IContext context, string name = null)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));

            var types = new List<Type>(WalkAssignableTypes(context));
            if (!types.Any())
                throw new Exception($"Could not determine context type for object of type {context.GetType().FullName}");

            types.ForEach(x => contexts[(x, name)] = context);
        }

        public bool TryGetContext<T>(out T context) where T : IContext
        {
            return TryGetContext(default, out context);
        }

        public bool TryGetContext<T>(string name, out T context) where T : IContext
        {
            IContext cachedContext;
            if (contexts.TryGetValue((typeof(T), name), out cachedContext) && cachedContext is T)
            {
                context = (T)cachedContext;
                return true;
            }

            context = default(T);
            return false;
        }

        public bool TryGetContext(Type type, out IContext context)
        {
            return TryGetContext(type, default, out context);
        }

        public bool TryGetContext(Type type, string name, out IContext context)
        {
            IContext cachedContext;
            if (contexts.TryGetValue((type, name), out cachedContext) && type.IsInstanceOfType(cachedContext))
            {
                context = cachedContext;
                return true;
            }

            context = null;
            return false;
        }
    }
}