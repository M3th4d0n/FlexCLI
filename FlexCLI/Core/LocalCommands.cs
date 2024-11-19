using System;
using System.Linq;
using System.IO.Compression;
using System.Security.Cryptography;

namespace FlexCLI.Core
{
    public static class LocalCommands
    {
        public static void Register()
        {
            CommandRegistry.Register("help", new Command(
                "help",
                "Displays information about available commands and plugins.",
                args =>
                {
                    if (args.Length == 0)
                    {
                        foreach (var command in CommandRegistry.GetAllCommands())
                        {
                            Console.WriteLine($"- {command.Name}");
                        }

                        Console.WriteLine("Use 'help -cl' for detailed command descriptions");
                    }
                    else if (args[0].Equals("-cl", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Plugins and their commands:");
                        var groupedCommands = CommandRegistry.GetAllCommands()
                            .GroupBy(command =>
                            {

                                var fullName = command.Name;
                                return fullName.Contains(".")
                                    ? fullName.Split('.')[0]
                                    : "core";
                            }, StringComparer.OrdinalIgnoreCase);

                        foreach (var group in groupedCommands)
                        {
                            Console.WriteLine($"Plugin: {group.Key}");
                            foreach (var command in group)
                            {

                                var shortName = command.Name.Contains(".")
                                    ? command.Name.Split('.')[1]
                                    : command.Name;

                                Console.WriteLine($"  - {shortName}: {command.Description}");
                            }
                        }
                    }


                    else
                    {
                        Console.WriteLine($"Unknown option: {args[0]}");
                    }
                }
            ));
            CommandRegistry.Register("cls", new Command(
                "cls",
                "Clear console",
                args =>
                {
                    Console.Clear();
                }));
            CommandRegistry.Register("dir", new Command(
                "dir",
                "Lists all files and directories in the current directory.",
                args =>
                {
                    var path = args.Length > 0 ? args[0] : Directory.GetCurrentDirectory();

                    if (!Directory.Exists(path))
                    {
                        Console.WriteLine($"Directory not found: {path}");
                        return;
                    }

                    var entries = Directory.GetFileSystemEntries(path);
                    foreach (var entry in entries)
                    {
                        Console.WriteLine(Path.GetFileName(entry)); // Вывод только имени файла или директории
                    }
                }
            ));
            CommandRegistry.Register("date", new Command(
                "date",
                "Displays the current system date.",
                args =>
                {
                    Console.WriteLine($"Current date: {DateTime.Now.ToLongDateString()}");
                }
            ));
            CommandRegistry.Register("time", new Command(
                "time",
                "Displays the current system time.",
                args =>
                {
                    Console.WriteLine($"Current time: {DateTime.Now.ToLongTimeString()}");
                }
            ));
            CommandRegistry.Register("echo", new Command(
                "echo",
                "Prints the input text to the console.",
                args =>
                {
                    Console.WriteLine(string.Join(" ", args));
                }
            ));
            CommandRegistry.Register("cat", new Command(
                "cat",
                "Displays the contents of a text file.",
                args =>
                {
                    if (args.Length == 0)
                    {
                        Console.WriteLine("Usage: type <file_name>");
                        return;
                    }

                    var fileName = args[0];
                    if (!File.Exists(fileName))
                    {
                        Console.WriteLine($"File not found: {fileName}");
                        return;
                    }

                    var contents = File.ReadAllText(fileName);
                    Console.WriteLine(contents);
                }
            ));
            CommandRegistry.Register("copy", new Command(
                "copy",
                "Copies a file to a new location.",
                args =>
                {
                    if (args.Length < 2)
                    {
                        Console.WriteLine("Usage: copy <source_file> <destination_file>");
                        return;
                    }

                    var sourceFile = args[0];
                    var destinationFile = args[1];

                    if (!File.Exists(sourceFile))
                    {
                        Console.WriteLine($"Source file not found: {sourceFile}");
                        return;
                    }

                    File.Copy(sourceFile, destinationFile, overwrite: true);
                    Console.WriteLine($"File copied from {sourceFile} to {destinationFile}");
                }
            ));
            CommandRegistry.Register("del", new Command(
                "del",
                "Deletes a file.",
                args =>
                {
                    if (args.Length == 0)
                    {
                        Console.WriteLine("Usage: del <file_name>");
                        return;
                    }

                    var fileName = args[0];
                    if (File.Exists(fileName))
                    {
                        File.Delete(fileName);
                        Console.WriteLine($"File deleted: {fileName}");
                    }
                    else
                    {
                        Console.WriteLine($"File not found: {fileName}");
                    }
                }
            ));
            CommandRegistry.Register("cd", new Command(
                "cd",
                "Changes the current directory.",
                args =>
                {
                    if (args.Length == 0)
                    {
                        Console.WriteLine($"Current directory: {Directory.GetCurrentDirectory()}");
                        return;
                    }

                    var newPath = args[0];
                    if (Directory.Exists(newPath))
                    {
                        Directory.SetCurrentDirectory(newPath);
                        Console.WriteLine($"Directory changed to: {newPath}");
                    }
                    else
                    {
                        Console.WriteLine($"Directory not found: {newPath}");
                    }
                }
            ));
            CommandRegistry.Register("ps", new Command(
                "ps",
                "Displays the list of running processes.",
                args =>
                {
                    var processes = System.Diagnostics.Process.GetProcesses();
                    foreach (var process in processes)
                    {
                        Console.WriteLine($"{process.Id}: {process.ProcessName}");
                    }
                }
            ));
            CommandRegistry.Register("kill", new Command(
                "kill",
                "Terminates a process by its ID.",
                args =>
                {
                    if (args.Length == 0)
                    {
                        Console.WriteLine("Usage: kill <process_id>");
                        return;
                    }

                    if (int.TryParse(args[0], out int processId))
                    {
                        try
                        {
                            var process = System.Diagnostics.Process.GetProcessById(processId);
                            process.Kill();
                            Console.WriteLine($"Process {processId} terminated.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid process ID.");
                    }
                }
            ));
            CommandRegistry.Register("hostname", new Command(
                "hostname",
                "Displays the hostname of the current machine.",
                args =>
                {
                    Console.WriteLine(Environment.MachineName);
                }
            ));
            CommandRegistry.Register("os", new Command(
                "os",
                "Displays information about the operating system.",
                args =>
                {
                    Console.WriteLine($"OS: {Environment.OSVersion}");
                    Console.WriteLine($"64-bit OS: {Environment.Is64BitOperatingSystem}");
                }
            ));
            CommandRegistry.Register("ping", new Command(
                "ping",
                "Sends a ping request to a specified host.",
                args =>
                {
                    if (args.Length == 0)
                    {
                        Console.WriteLine("Usage: ping <hostname>");
                        return;
                    }

                    var ping = new System.Net.NetworkInformation.Ping();
                    try
                    {
                        var reply = ping.Send(args[0]);
                        if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                        {
                            Console.WriteLine($"Ping to {args[0]} successful. Time: {reply.RoundtripTime}ms");
                        }
                        else
                        {
                            Console.WriteLine($"Ping to {args[0]} failed. Status: {reply.Status}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                }
            ));
            CommandRegistry.Register("tree", new Command(
                "tree",
                "Displays the directory structure starting from the current or specified directory.",
                args =>
                {
                    var path = args.Length > 0 ? args[0] : Directory.GetCurrentDirectory();

                    void PrintTree(string directory, string indent)
                    {
                        Console.WriteLine($"{indent}{Path.GetFileName(directory)} [DIR]");
                        foreach (var dir in Directory.GetDirectories(directory))
                        {
                            PrintTree(dir, indent + "  ");
                        }
                        foreach (var file in Directory.GetFiles(directory))
                        {
                            Console.WriteLine($"{indent}  {Path.GetFileName(file)}");
                        }
                    }

                    if (Directory.Exists(path))
                    {
                        PrintTree(path, "");
                    }
                    else
                    {
                        Console.WriteLine($"Directory not found: {path}");
                    }
                }
            ));
            CommandRegistry.Register("whoami", new Command(
                "whoami",
                "Displays the current user.",
                args =>
                {
                    Console.WriteLine(Environment.UserName);
                }
            ));
            CommandRegistry.Register("uptime", new Command(
                "uptime",
                "Displays the system uptime.",
                args =>
                {
                    var uptime = TimeSpan.FromMilliseconds(Environment.TickCount64);
                    Console.WriteLine($"System uptime: {uptime.Days} days {uptime.Hours} hours {uptime.Minutes} minutes");
                }
            ));
            CommandRegistry.Register("mv", new Command(
                "mv",
                "Moves a file or directory to a new location.",
                args =>
                {
                    if (args.Length < 2)
                    {
                        Console.WriteLine("Usage: mv <source> <destination>");
                        return;
                    }

                    var source = args[0];
                    var destination = args[1];

                    if (File.Exists(source))
                    {
                        File.Move(source, destination, overwrite: true);
                        Console.WriteLine($"File moved from {source} to {destination}");
                    }
                    else if (Directory.Exists(source))
                    {
                        Directory.Move(source, destination);
                        Console.WriteLine($"Directory moved from {source} to {destination}");
                    }
                    else
                    {
                        Console.WriteLine($"Source not found: {source}");
                    }
                }
            ));
            CommandRegistry.Register("touch", new Command(
                "touch",
                "Creates an empty file.",
                args =>
                {
                    if (args.Length == 0)
                    {
                        Console.WriteLine("Usage: touch <file_name>");
                        return;
                    }

                    var fileName = args[0];
                    if (!File.Exists(fileName))
                    {
                        File.Create(fileName).Close();
                        Console.WriteLine($"File created: {fileName}");
                    }
                    else
                    {
                        Console.WriteLine($"File already exists: {fileName}");
                    }
                }
            ));
            CommandRegistry.Register("head", new Command(
                "head",
                "Displays the first N lines of a file (default is 10).",
                args =>
                {
                    if (args.Length == 0)
                    {
                        Console.WriteLine("Usage: head <file_name> [lines]");
                        return;
                    }

                    var fileName = args[0];
                    var lines = args.Length > 1 && int.TryParse(args[1], out int count) ? count : 10;

                    if (File.Exists(fileName))
                    {
                        var fileLines = File.ReadLines(fileName).Take(lines);
                        foreach (var line in fileLines)
                        {
                            Console.WriteLine(line);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"File not found: {fileName}");
                    }
                }
            ));
            CommandRegistry.Register("tail", new Command(
                "tail",
                "Displays the last N lines of a file (default is 10).",
                args =>
                {
                    if (args.Length == 0)
                    {
                        Console.WriteLine("Usage: tail <file_name> [lines]");
                        return;
                    }

                    var fileName = args[0];
                    var lines = args.Length > 1 && int.TryParse(args[1], out int count) ? count : 10;

                    if (File.Exists(fileName))
                    {
                        var fileLines = File.ReadLines(fileName).Reverse().Take(lines).Reverse();
                        foreach (var line in fileLines)
                        {
                            Console.WriteLine(line);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"File not found: {fileName}");
                    }
                }
            ));
            CommandRegistry.Register("env", new Command(
                "env",
                "Displays environment variables.",
                args =>
                {
                    foreach (var env in Environment.GetEnvironmentVariables().Keys)
                    {
                        Console.WriteLine($"{env}={Environment.GetEnvironmentVariables()[env]}");
                    }
                }
            ));
            CommandRegistry.Register("find", new Command(
                "find",
                "Searches for files and directories by name in the current directory.",
                args =>
                {
                    if (args.Length == 0)
                    {
                        Console.WriteLine("Usage: find <name>");
                        return;
                    }

                    var searchTerm = args[0];
                    var currentDir = Directory.GetCurrentDirectory();

                    var files = Directory.GetFiles(currentDir, $"*{searchTerm}*");
                    var directories = Directory.GetDirectories(currentDir, $"*{searchTerm}*");

                    foreach (var dir in directories)
                    {
                        Console.WriteLine($"{Path.GetFileName(dir)} [DIR]");
                    }

                    foreach (var file in files)
                    {
                        Console.WriteLine(Path.GetFileName(file));
                    }
                }
            ));
            CommandRegistry.Register("size", new Command(
                "size",
                "Displays the size of a file or directory.",
                args =>
                {
                    if (args.Length == 0)
                    {
                        Console.WriteLine("Usage: size <file_or_directory>");
                        return;
                    }

                    var path = args[0];
                    if (File.Exists(path))
                    {
                        var size = new FileInfo(path).Length;
                        Console.WriteLine($"File size: {size} bytes");
                    }
                    else if (Directory.Exists(path))
                    {
                        long GetDirectorySize(string dir)
                        {
                            return Directory.GetFiles(dir, "*", SearchOption.AllDirectories)
                                .Sum(file => new FileInfo(file).Length);
                        }

                        var size = GetDirectorySize(path);
                        Console.WriteLine($"Directory size: {size} bytes");
                    }
                    else
                    {
                        Console.WriteLine($"Path not found: {path}");
                    }
                }
            ));
            CommandRegistry.Register("rename", new Command(
                "rename",
                "Renames a file or directory.",
                args =>
                {
                    if (args.Length < 2)
                    {
                        Console.WriteLine("Usage: rename <old_name> <new_name>");
                        return;
                    }

                    var oldName = args[0];
                    var newName = args[1];

                    if (File.Exists(oldName))
                    {
                        File.Move(oldName, newName);
                        Console.WriteLine($"File renamed from {oldName} to {newName}");
                    }
                    else if (Directory.Exists(oldName))
                    {
                        Directory.Move(oldName, newName);
                        Console.WriteLine($"Directory renamed from {oldName} to {newName}");
                    }
                    else
                    {
                        Console.WriteLine($"Path not found: {oldName}");
                    }
                }
            ));
            CommandRegistry.Register("diskinfo", new Command(
                "diskinfo",
                "Displays information about disk drives.",
                args =>
                {
                    foreach (var drive in DriveInfo.GetDrives())
                    {
                        if (drive.IsReady)
                        {
                            Console.WriteLine($"Drive {drive.Name}");
                            Console.WriteLine($"  Total Size: {drive.TotalSize / (1024 * 1024 * 1024)} GB");
                            Console.WriteLine($"  Free Space: {drive.AvailableFreeSpace / (1024 * 1024 * 1024)} GB");
                            Console.WriteLine($"  Drive Format: {drive.DriveFormat}");
                        }
                    }
                }
            ));
            CommandRegistry.Register("cpuinfo", new Command(
                "cpuinfo",
                "Displays information about the CPU.",
                args =>
                {
                    Console.WriteLine($"Processor Count: {Environment.ProcessorCount}");
                    Console.WriteLine($"64-bit Processor: {Environment.Is64BitProcess}");
                }
            ));
            CommandRegistry.Register("ipconfig", new Command(
                "ipconfig",
                "Displays network interface information.",
                args =>
                {
                    var interfaces = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
                    foreach (var netInterface in interfaces)
                    {
                        Console.WriteLine($"Interface: {netInterface.Name}");
                        Console.WriteLine($"  Status: {netInterface.OperationalStatus}");
                        Console.WriteLine($"  MAC Address: {netInterface.GetPhysicalAddress()}");
                        var ipProps = netInterface.GetIPProperties();
                        foreach (var ip in ipProps.UnicastAddresses)
                        {
                            Console.WriteLine($"  IP Address: {ip.Address}");
                        }
                    }
                }
            ));
            CommandRegistry.Register("download", new Command(
                "download",
                "Downloads a file from the specified URL.",
                args =>
                {
                    if (args.Length < 2)
                    {
                        Console.WriteLine("Usage: download <url> <destination>");
                        return;
                    }

                    var url = args[0];
                    var destination = args[1];

                    using var client = new System.Net.WebClient();
                    try
                    {
                        client.DownloadFile(url, destination);
                        Console.WriteLine($"File downloaded to: {destination}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error downloading file: {ex.Message}");
                    }
                }
            ));
            CommandRegistry.Register("calc", new Command(
                "calc",
                "Performs simple arithmetic calculations.",
                args =>
                {
                    if (args.Length < 3)
                    {
                        Console.WriteLine("Usage: calc <number1> <operator> <number2>");
                        return;
                    }

                    if (double.TryParse(args[0], out double num1) &&
                        double.TryParse(args[2], out double num2))
                    {
                        var op = args[1];
                        double result = op switch
                        {
                            "+" => num1 + num2,
                            "-" => num1 - num2,
                            "*" => num1 * num2,
                            "/" => num2 != 0 ? num1 / num2 : double.NaN,
                            _ => double.NaN
                        };

                        Console.WriteLine($"Result: {result}");
                    }
                    else
                    {
                        Console.WriteLine("Invalid numbers.");
                    }
                }
            ));
            CommandRegistry.Register("users", new Command(
                "users",
                "Displays a list of users on the system.",
                args =>
                {
                    var usersPath = @"C:\Users";
                    if (Directory.Exists(usersPath))
                    {
                        var users = Directory.GetDirectories(usersPath).Select(Path.GetFileName);
                        foreach (var user in users)
                        {
                            Console.WriteLine(user);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Unable to retrieve users.");
                    }
                }
            ));
            CommandRegistry.Register("attrib", new Command(
                "attrib",
                "Displays or changes the attributes of a file or directory.",
                args =>
                {
                    if (args.Length == 0)
                    {
                        Console.WriteLine("Usage: attrib <file_path> [+|-][RASH]");
                        return;
                    }

                    var path = args[0];
                    if (!File.Exists(path) && !Directory.Exists(path))
                    {
                        Console.WriteLine($"File or directory not found: {path}");
                        return;
                    }

                    var attributes = File.GetAttributes(path);
                    if (args.Length == 1)
                    {
                        Console.WriteLine($"Attributes: {attributes}");
                        return;
                    }

                    foreach (var command in args.Skip(1))
                    {
                        switch (command)
                        {
                            case "+R": attributes |= FileAttributes.ReadOnly; break;
                            case "-R": attributes &= ~FileAttributes.ReadOnly; break;
                            case "+A": attributes |= FileAttributes.Archive; break;
                            case "-A": attributes &= ~FileAttributes.Archive; break;
                            case "+S": attributes |= FileAttributes.System; break;
                            case "-S": attributes &= ~FileAttributes.System; break;
                            case "+H": attributes |= FileAttributes.Hidden; break;
                            case "-H": attributes &= ~FileAttributes.Hidden; break;
                            default:
                                Console.WriteLine($"Unknown attribute command: {command}");
                                break;
                        }
                    }

                    File.SetAttributes(path, attributes);
                    Console.WriteLine($"Updated attributes: {attributes}");
                }
            ));
            CommandRegistry.Register("xcopy", new Command(
                "xcopy",
                "Copies files and directories, including subdirectories.",
                args =>
                {
                    if (args.Length < 2)
                    {
                        Console.WriteLine("Usage: xcopy <source> <destination> [/S]");
                        return;
                    }

                    var source = args[0];
                    var destination = args[1];
                    var recursive = args.Length > 2 && args[2].Equals("/S", StringComparison.OrdinalIgnoreCase);

                    if (!Directory.Exists(source))
                    {
                        Console.WriteLine($"Source directory not found: {source}");
                        return;
                    }

                    void CopyDirectory(string sourceDir, string destinationDir, bool copySubDirs)
                    {
                        Directory.CreateDirectory(destinationDir);
                        foreach (var file in Directory.GetFiles(sourceDir))
                        {
                            var destFile = Path.Combine(destinationDir, Path.GetFileName(file));
                            File.Copy(file, destFile, overwrite: true);
                        }
                        if (copySubDirs)
                        {
                            foreach (var dir in Directory.GetDirectories(sourceDir))
                            {
                                var destDir = Path.Combine(destinationDir, Path.GetFileName(dir));
                                CopyDirectory(dir, destDir, copySubDirs);
                            }
                        }
                    }

                    CopyDirectory(source, destination, recursive);
                    Console.WriteLine($"Copied from {source} to {destination}");
                }
            ));
            CommandRegistry.Register("path", new Command(
                "path",
                "Displays or modifies the system PATH variable.",
                args =>
                {
                    var path = Environment.GetEnvironmentVariable("PATH");
                    if (args.Length == 0)
                    {
                        Console.WriteLine($"Current PATH: {path}");
                        return;
                    }

                    if (args[0] == "add" && args.Length > 1)
                    {
                        var newPath = $"{path};{args[1]}";
                        Environment.SetEnvironmentVariable("PATH", newPath);
                        Console.WriteLine($"Added to PATH: {args[1]}");
                    }
                    else if (args[0] == "remove" && args.Length > 1)
                    {
                        var newPath = string.Join(';', path.Split(';').Where(p => !p.Equals(args[1], StringComparison.OrdinalIgnoreCase)));
                        Environment.SetEnvironmentVariable("PATH", newPath);
                        Console.WriteLine($"Removed from PATH: {args[1]}");
                    }
                    else
                    {
                        Console.WriteLine("Usage: path [add|remove] <path>");
                    }
                }
            ));
            CommandRegistry.Register("netstat", new Command(
                "netstat",
                "Displays active network connections.",
                args =>
                {
                    var properties = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties();
                    var connections = properties.GetActiveTcpConnections();
                    foreach (var connection in connections)
                    {
                        Console.WriteLine($"Local: {connection.LocalEndPoint} | Remote: {connection.RemoteEndPoint} | State: {connection.State}");
                    }
                }
            ));

            CommandRegistry.Register("chk", new Command(
                "chk",
                "Checks if a file or directory exists.",
                args =>
                {
                    if (args.Length == 0)
                    {
                        Console.WriteLine("Usage: chk <path>");
                        return;
                    }

                    var path = args[0];
                    if (File.Exists(path))
                    {
                        Console.WriteLine($"File exists: {path}");
                    }
                    else if (Directory.Exists(path))
                    {
                        Console.WriteLine($"Directory exists: {path}");
                    }
                    else
                    {
                        Console.WriteLine($"Path not found: {path}");
                    }
                }
            ));
            CommandRegistry.Register("findstr", new Command(
                "findstr",
                "Searches for a specific string in one or more files.",
                args =>
                {
                    if (args.Length < 2)
                    {
                        Console.WriteLine("Usage: findstr <search_term> <file1> [file2]...");
                        return;
                    }

                    var searchTerm = args[0];
                    var files = args.Skip(1);

                    foreach (var file in files)
                    {
                        if (File.Exists(file))
                        {
                            var lines = File.ReadLines(file)
                                .Select((line, index) => (line, index))
                                .Where(pair => pair.line.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));

                            foreach (var (line, index) in lines)
                            {
                                Console.WriteLine($"{file}({index + 1}): {line}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"File not found: {file}");
                        }
                    }
                }
            ));
            CommandRegistry.Register("compress", new Command(
                "compress",
                "Compresses files into a ZIP archive.",
                args =>
                {
                    if (args.Length < 2)
                    {
                        Console.WriteLine("Usage: compress <output.zip> <file1> [file2]...");
                        return;
                    }

                    var zipFile = args[0];
                    var files = args.Skip(1);

                    using var archive = ZipFile.Open(zipFile, ZipArchiveMode.Create);
                    foreach (var file in files)
                    {
                        if (File.Exists(file))
                        {
                            archive.CreateEntryFromFile(file, Path.GetFileName(file));
                            Console.WriteLine($"Compressed: {file}");
                        }
                        else
                        {
                            Console.WriteLine($"File not found: {file}");
                        }
                    }
                }
            ));

            CommandRegistry.Register("decompress", new Command(
                "decompress",
                "Extracts files from a ZIP archive.",
                args =>
                {
                    if (args.Length < 2)
                    {
                        Console.WriteLine("Usage: decompress <archive.zip> <destination>");
                        return;
                    }

                    var zipFile = args[0];
                    var destination = args[1];

                    if (File.Exists(zipFile))
                    {
                        ZipFile.ExtractToDirectory(zipFile, destination);
                        Console.WriteLine($"Extracted to {destination}");
                    }
                    else
                    {
                        Console.WriteLine($"Archive not found: {zipFile}");
                    }
                }
            ));
            CommandRegistry.Register("hash", new Command(
                "hash",
                "Calculates the hash of a file. Usage: hash <file> [md5|sha1|sha256]",
                args =>
                {
                    if (args.Length < 1)
                    {
                        Console.WriteLine("Usage: hash <file> [md5|sha1|sha256]");
                        return;
                    }

                    var filePath = args[0];
                    var algorithm = args.Length > 1 ? args[1].ToLower() : "md5";

                    if (!File.Exists(filePath))
                    {
                        Console.WriteLine($"File not found: {filePath}");
                        return;
                    }

                    using var stream = File.OpenRead(filePath);
                    using HashAlgorithm hashAlgorithm = algorithm switch
                    {
                        "md5" => MD5.Create(),
                        "sha1" => SHA1.Create(),
                        "sha256" => SHA256.Create(),
                        _ => throw new ArgumentException($"Unsupported algorithm: {algorithm}")
                    };

                    var hash = hashAlgorithm.ComputeHash(stream);
                    Console.WriteLine($"{algorithm.ToUpper()} hash: {BitConverter.ToString(hash).Replace("-", "").ToLower()}");
                }
            ));
            CommandRegistry.Register("whoami /priv", new Command(
                "whoami /priv",
                "Displays the current user's privileges.",
                args =>
                {
                    Console.WriteLine("User privileges:");
                    Console.WriteLine("- Administrator: " + (Environment.UserInteractive ? "Yes" : "No"));
                    Console.WriteLine("- Debug Privileges: N/A (requires advanced checks)");
                }
            ));
            CommandRegistry.Register("sysinfo", new Command(
                "sysinfo",
                "Displays detailed system information.",
                args =>
                {
                    Console.WriteLine($"OS: {Environment.OSVersion}");
                    Console.WriteLine($"Machine Name: {Environment.MachineName}");
                    Console.WriteLine($"Processor Count: {Environment.ProcessorCount}");
                    Console.WriteLine($"System Directory: {Environment.SystemDirectory}");
                    Console.WriteLine($"User: {Environment.UserName}");
                    Console.WriteLine($"64-bit OS: {Environment.Is64BitOperatingSystem}");
                }
            ));

            CommandRegistry.Register("resolve", new Command(
                "resolve",
                "Resolves a hostname to an IP address.",
                args =>
                {
                    if (args.Length == 0)
                    {
                        Console.WriteLine("Usage: resolve <hostname>");
                        return;
                    }

                    try
                    {
                        var host = System.Net.Dns.GetHostEntry(args[0]);
                        foreach (var ip in host.AddressList)
                        {
                            Console.WriteLine(ip);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                }
            ));
            CommandRegistry.Register("scanport", new Command(
                "scanport",
                "Checks if a specific port is open on a host.",
                args =>
                {
                    if (args.Length < 2)
                    {
                        Console.WriteLine("Usage: scanport <hostname> <port>");
                        return;
                    }

                    var host = args[0];
                    if (!int.TryParse(args[1], out int port))
                    {
                        Console.WriteLine("Invalid port number.");
                        return;
                    }

                    try
                    {
                        using var client = new System.Net.Sockets.TcpClient();
                        var result = client.BeginConnect(host, port, null, null);
                        var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(2));
                        if (success)
                        {
                            Console.WriteLine($"Port {port} on {host} is open.");
                        }
                        else
                        {
                            Console.WriteLine($"Port {port} on {host} is closed.");
                        }
                    }
                    catch
                    {
                        Console.WriteLine($"Failed to connect to {host}:{port}");
                    }
                }
            ));
            CommandRegistry.Register("jsonlint", new Command(
                "jsonlint",
                "Validates the syntax of a JSON string or file.",
                args =>
                {
                    if (args.Length == 0)
                    {
                        Console.WriteLine("Usage: jsonlint <json_string|file>");
                        return;
                    }

                    var input = args[0];
                    string json;

                    if (File.Exists(input))
                    {
                        json = File.ReadAllText(input);
                    }
                    else
                    {
                        json = input;
                    }

                    try
                    {
                        var parsed = System.Text.Json.JsonDocument.Parse(json);
                        Console.WriteLine("JSON is valid.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Invalid JSON: {ex.Message}");
                    }
                }
            ));
            CommandRegistry.Register("listzip", new Command(
                "listzip",
                "Lists the contents of a ZIP archive.",
                args =>
                {
                    if (args.Length == 0)
                    {
                        Console.WriteLine("Usage: listzip <archive>");
                        return;
                    }

                    var archive = args[0];
                    if (!File.Exists(archive))
                    {
                        Console.WriteLine($"Archive not found: {archive}");
                        return;
                    }

                    using var zip = ZipFile.OpenRead(archive);
                    foreach (var entry in zip.Entries)
                    {
                        Console.WriteLine($"{entry.FullName} ({entry.Length} bytes)");
                    }
                }
            ));
            CommandRegistry.Register("xmlpretty", new Command(
                "xmlpretty",
                "Formats XML to be human-readable.",
                args =>
                {
                    if (args.Length == 0)
                    {
                        Console.WriteLine("Usage: xmlpretty <xml_string|file>");
                        return;
                    }

                    var input = args[0];
                    string xml;

                    if (File.Exists(input))
                    {
                        xml = File.ReadAllText(input);
                    }
                    else
                    {
                        xml = input;
                    }

                    try
                    {
                        var doc = new System.Xml.XmlDocument();
                        doc.LoadXml(xml);
                        var stringBuilder = new System.Text.StringBuilder();
                        using (var writer = System.Xml.XmlWriter.Create(stringBuilder, new System.Xml.XmlWriterSettings { Indent = true }))
                        {
                            doc.Save(writer);
                        }
                        Console.WriteLine(stringBuilder.ToString());
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Invalid XML: {ex.Message}");
                    }
                }
            ));
CommandRegistry.Register("curl", new Command(
    "curl",
    "Performs an HTTP request. Usage: curl <method> <url> [options]",
    args =>
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Usage: curl <method> <url> [--header key:value] [--data 'body']");
            return;
        }

        var method = args[0].ToUpper();
        var url = args[1];
        var headers = new Dictionary<string, string>();
        string body = null;

        // Парсим дополнительные аргументы
        for (int i = 2; i < args.Length; i++)
        {
            if (args[i].Equals("--header", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length)
            {
                var header = args[++i].Split(':', 2);
                if (header.Length == 2)
                {
                    headers[header[0].Trim()] = header[1].Trim();
                }
            }
            else if (args[i].Equals("--data", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length)
            {
                body = args[++i];
            }
        }

        using var client = new System.Net.Http.HttpClient();

        // Устанавливаем заголовки
        foreach (var header in headers)
        {
            client.DefaultRequestHeaders.Add(header.Key, header.Value);
        }

        try
        {
            System.Net.Http.HttpResponseMessage response;
            if (method == "POST")
            {
                var content = new System.Net.Http.StringContent(body ?? "", System.Text.Encoding.UTF8, "application/json");
                response = client.PostAsync(url, content).Result;
            }
            else if (method == "GET")
            {
                response = client.GetAsync(url).Result;
            }
            else
            {
                Console.WriteLine($"Unsupported method: {method}");
                return;
            }

            var result = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine($"Status Code: {response.StatusCode}");
            Console.WriteLine("Response Headers:");
            foreach (var header in response.Headers)
            {
                Console.WriteLine($"  {header.Key}: {string.Join(", ", header.Value)}");
            }
            Console.WriteLine("\nResponse Body:");
            Console.WriteLine(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"HTTP error: {ex.Message}");
        }
    }
));

        }
    }
}
