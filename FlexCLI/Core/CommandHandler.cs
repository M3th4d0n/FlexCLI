using System;
using System.Collections.Generic;

namespace FlexCLI.Core
{
    public static class CommandRegistry
    {
        private static readonly Dictionary<string, Command> _commands = 
            new(StringComparer.OrdinalIgnoreCase);
        
        public static void Register(string commandName, Command command)
        {
            if (_commands.ContainsKey(commandName))
            {
                return;
            }

            _commands[commandName] = command;
        }
        
        public static bool Execute(string input, string[] commandArguments)
        {
            if (_commands.TryGetValue(input, out var command))
            {
                command.Execute(commandArguments);
                return true;
            }
            Console.WriteLine($"Command {input} not found.");
            return false;
        }
        
        public static IEnumerable<Command> GetAllCommands()
        {
            return _commands.Values;
        }
    }
}