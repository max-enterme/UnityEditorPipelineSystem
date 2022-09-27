using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace UnityEditorPipelineSystem.Core
{
    public interface ICommandLineArgumentContainer : IContext
    {
        public IReadOnlyDictionary<string, string> GetOptions();

        public bool ContainsOption(string optionName);
        public string GetOptionValue(string optionName);
        public bool TryGetOptionValue(string optionName, out string optionValue);
    }

    public class CommandLineArgumentContainer : ICommandLineArgumentContainer
    {
        public static CommandLineArgumentContainer Create()
        {
            var args = Environment.GetCommandLineArgs();
            var container = new CommandLineArgumentContainer();

            for (var i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith("-"))
                {
                    container.options.Add(args[i][1..], args.ElementAtOrDefault(i + 1));
                }
            }

            return container;
        }

        private readonly IDictionary<string, string> options = new Dictionary<string, string>();

        private CommandLineArgumentContainer()
        {
        }

        public IReadOnlyDictionary<string, string> GetOptions()
        {
            return new ReadOnlyDictionary<string, string>(options);
        }

        public bool ContainsOption(string optionName)
        {
            return options.ContainsKey(optionName);
        }

        public string GetOptionValue(string optionName)
        {
            return options[optionName];
        }

        public string GetOrDefaultOptionValue(string optionName)
        {
            return options.TryGetValue(optionName, out var optionValue) ? optionValue : default;
        }

        public bool TryGetOptionValue(string optionName, out string optionValue)
        {
            return options.TryGetValue(optionName, out optionValue);
        }
    }
}
