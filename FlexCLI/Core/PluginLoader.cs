using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FlexCLI.Core
{
    public static class PluginLoader
    {
        public static void LoadPlugins(string pluginDirectory)
{
    if (!Directory.Exists(pluginDirectory))
    {
        Directory.CreateDirectory(pluginDirectory);
        Console.WriteLine($"The plugin directory has been created: {pluginDirectory}");
        return;
    }

    var dllFiles = Directory.GetFiles(pluginDirectory, "*.dll");

    foreach (var dll in dllFiles)
    {
        try
        {
            var assembly = Assembly.LoadFrom(dll);
            var assemblyName = assembly.GetName().Name; 

            var mainType = assembly.GetTypes().FirstOrDefault(t => t.Name == "Main");
            if (mainType != null)
            {
                Activator.CreateInstance(mainType);

                var commandsField = mainType.GetField("Commands", BindingFlags.Public | BindingFlags.Static);
                if (commandsField == null) continue;

                var commandsValue = commandsField.GetValue(null);
                if (commandsValue == null) continue;

                if (commandsValue is IDictionary commandsDict)
                {
                    foreach (DictionaryEntry commandEntry in commandsDict)
                    {
                        var commandObj = commandEntry.Value;
                        if (commandObj == null) continue;

                        var nameProperty = commandObj.GetType().GetProperty("Name");
                        var descriptionProperty = commandObj.GetType().GetProperty("Description");
                        var executeProperty = commandObj.GetType().GetProperty("Execute");

                        if (nameProperty == null || descriptionProperty == null || executeProperty == null)
                            continue;

                        var name = nameProperty.GetValue(commandObj)?.ToString();
                        var description = descriptionProperty.GetValue(commandObj)?.ToString();
                        var executeDelegate = executeProperty.GetValue(commandObj) as Action<string[]>;

                        if (name != null && description != null && executeDelegate != null)
                        {
                            CommandRegistry.Register($"{assembly.GetName().Name}.{name}", new Command(
                                $"{assembly.GetName().Name}.{name}",
                                description,
                                executeDelegate
                            ));

                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading plugin from {dll}: {ex.Message}");
        }
    }
}

    }
}
