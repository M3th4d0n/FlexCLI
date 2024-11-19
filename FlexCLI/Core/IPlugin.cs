public class Command
{
    public string Name { get; }
    public string Description { get; }
    public Action<string[]> Execute { get; }
    public string PluginName { get; } // Новый параметр

    public Command(string name, string description, Action<string[]> execute, string pluginName = "core")
    {
        Name = name;
        Description = description;
        Execute = execute;
        PluginName = pluginName;
    }
}