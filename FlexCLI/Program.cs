using FlexCLI.Core;

var pluginDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Plugins");
PluginLoader.LoadPlugins(pluginDirectory);

// Register help command
LocalCommands.Register();

Console.WriteLine("FlexCLI is loaded. Use <PluginName>.<CommandName>");

while (true)
{
    Console.Write($"{Directory.GetCurrentDirectory()}> ");
    var input = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(input)) continue;

    if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("Goodbye!");
        break;
    }

    var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    var command = parts[0];
    var arguments = parts.Skip(1).ToArray();

    if (!CommandRegistry.Execute(command, arguments))
    {
        
    }
}