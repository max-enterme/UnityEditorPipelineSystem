using System;

namespace UnityEditorPipelineSystem.Core
{
    public interface IContextContainer
    {
        bool ContainsContext<T>(string name = default) where T : IContext;
        bool ContainsContext(Type type, string name = default);

        T GetContext<T>(string name = default) where T : IContext;
        IContext GetContext(Type type, string name = default);

        void SetContext<T>(T context, string name = default) where T : IContext;
        void SetContext(Type type, IContext context, string name = default);
        void SetContext(IContext context, string name = default);

        bool TryGetContext<T>(out T context) where T : IContext;
        bool TryGetContext<T>(string name, out T context) where T : IContext;
        bool TryGetContext(Type type, out IContext context);
        bool TryGetContext(Type type, string name, out IContext context);
    }
}