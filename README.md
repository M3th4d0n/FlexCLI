
# FlexCLI

**FlexCLI** is a powerful and customizable CLI (Command Line Interface) console designed to simplify system interactions and extend functionality with plugins. It offers a wide range of built-in commands for file manipulation, process management, network operations, and much more.

---

## 🌟 Features

- **Rich Built-in Commands**: Includes system commands for file operations, networking, and utilities, similar to traditional Windows CMD or Linux shell commands.
- **Extensible with Plugins**: Easily extend the functionality by writing custom plugins.
- **Developer-Friendly**: Built with C#, enabling easy customization and integration.

---

## 🚀 Getting Started

### Prerequisites

- [.NET 6+ SDK](https://dotnet.microsoft.com/download) installed on your system.

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/FlexCLI.git
   cd FlexCLI
   ```

2. Build the project:
   ```bash
   dotnet build
   ```

3. Run the CLI:
   ```bash
   dotnet run
   ```

---

## 🛠 Built-in Commands

FlexCLI comes with a wide array of built-in commands. Here are some highlights:

| Command       | Description                                       |
|---------------|---------------------------------------------------|
| `dir`         | Lists files and directories in the current path. |
| `cd`          | Changes the current directory.                   |
| `copy`        | Copies a file to a new location.                 |
| `del`         | Deletes a specified file.                        |
| `ping`        | Sends a ping to a host to test connectivity.     |
| `curl`        | Performs HTTP requests (GET/POST).               |
| `tree`        | Displays directory structure in tree format.     |
| `uptime`      | Shows the system's uptime.                       |
| `compress`    | Creates a ZIP archive of files.                  |
| `decompress`  | Extracts files from a ZIP archive.               |
| `hash`        | Calculates MD5/SHA1/SHA256 hash of a file.       |

Run `help` for the full list of commands.

---

## 🔌 Writing Plugins

FlexCLI allows developers to create plugins as `.dll` files. Each plugin must include:

1. **An `Info` Class**:
   Contains metadata about the plugin, such as name, version, and author.
2. **A `Main` Class**:
   Responsible for registering commands provided by the plugin.

### Example Plugin

1. **Create a New C# Class Library**:
   ```bash
   dotnet new classlib -n MyPlugin
   cd MyPlugin
   ```

2. **Define the `Info` and `Main` Classes**:

   ```csharp
   using System;
   using System.Collections.Generic;

   // Plugin Metadata
   public class Info
   {
       public static string Name = "MyPlugin";
       public static string Version = "1.0.0";
       public static string Author = "Your Name";
   }

   // Plugin Entry Point
   public class Main
   {
       public static Dictionary<string, Command> Commands = new();

       public Main()
       {
           RegisterCommands();
       }

       private void RegisterCommands()
       {
           Commands["hello"] = new Command
           {
               Name = "hello",
               Description = "Displays a greeting message.",
               Execute = args =>
               {
                   Console.WriteLine("Hello from MyPlugin!");
               }
           };
       }
   }
   ```

3. **Build the Plugin**:
   ```bash
   dotnet build
   ```

4. **Install the Plugin**:
   - Locate the `.dll` file in the `bin/Debug/net6.0` folder.
   - Copy it to the `Plugins` directory of FlexCLI:
     ```plaintext
     FlexCLI/
     ├── Plugins/
     │   ├── MyPlugin.dll
     ```

5. **Run FlexCLI**:
   - Start FlexCLI, and the plugin will be automatically loaded.
   - Use `help --cl` to verify that your plugin is loaded.

---

## 🤝 Contributing

Contributions are welcome! If you have ideas for new features or commands, feel free to:

1. Fork the repository.
2. Create a new branch.
3. Submit a pull request.

---

## 📜 License

This project is licensed under the MIT License.
