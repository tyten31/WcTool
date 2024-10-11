using System.CommandLine;
using System.Reflection;

namespace WcTool
{
    internal class CommandManager
    {
        public string[] BuildCommand(string[] args)
        {
            if (args?.Length == 1)
            {
                return args[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            }
            else if (args?.Length == 2)
            {
                var list = new List<string>(args[1].Split(' ', StringSplitOptions.RemoveEmptyEntries))
                {
                    args[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).Last()
                };

                return list.ToArray();
            }

            return [];
        }

        public string[] BuildInvokeCommand(string[] args)
        {
            var invokeCommand = new List<string>();

            if (args.Length == 2)
            {
                invokeCommand.Add(args[1].Trim());
            }
            else if (args.Length == 3)
            {
                invokeCommand.Add("-o");
                invokeCommand.Add(args[1].Trim());
                invokeCommand.Add(args[2].Trim());
            }

            return invokeCommand.ToArray();
        }

        public RootCommand BuildRootCommand(string[] args)
        {
            var rootCommand = new RootCommand();

            if (args.Length == 3)
            {
                rootCommand.AddOption(new Option<string>(["--option", "-o"], "a flag option"));
            }

            rootCommand.AddArgument(new Argument<string>("file", "a required file argument"));

            return rootCommand;
        }

        public void ExecuteCommand(string option, string file)
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), file);
            var fileInfo = new FileInfo(path);

            switch (option)
            {
                case "-c":
                    Console.WriteLine($" {fileInfo.Length} {file}");
                    break;

                case "-l":
                    Console.WriteLine($" {GetNumberOfLines(path)} {file}");
                    break;

                case "-w":
                    Console.WriteLine($" {GetNumberOfWords(path)} {file}");
                    break;

                case "-m":
                    Console.WriteLine($" {GetNumberOfCharacters(path)} {file}");
                    break;

                default:
                    Console.WriteLine($" {GetNumberOfLines(path)} {GetNumberOfWords(path)} {fileInfo.Length} {file}");
                    break;
            }
        }

        public bool IsValidCommand(string[] args)
        {
            // Check length
            if (args == null || args.Length < 2 || args.Length > 3)
            {
                return false;
            }

            // Check that starts with ccwc
            if (!args[0].Trim().Equals("ccwc", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var options = new List<string> { "-c", "-l", "-w", "-m" };
            return args.Length != 3 || options.Any(args[1].Contains);
        }

        private int GetNumberOfCharacters(string path)
        {
            var characters = 0;

            if (File.Exists(path))
            {
                using StreamReader read = new(path);
                string? line;
                while ((line = read.ReadLine()) != null)
                {
                    characters += line.ToCharArray().Length;
                }
            }

            return characters;
        }

        private int GetNumberOfLines(string path)
        {
            var lines = 0;

            if (File.Exists(path))
            {
                using StreamReader read = new(path);
                string? line;
                while ((line = read.ReadLine()) != null)
                {
                    lines++;
                }
            }

            return lines;
        }

        private int GetNumberOfWords(string path)
        {
            var words = 0;

            if (File.Exists(path))
            {
                using StreamReader read = new(path);
                string? line;
                while ((line = read.ReadLine()) != null)
                {
                    bool inWord = false;

                    for (int i = 0; i < line.Length; i++)
                    {
                        bool isWhiteSpace = char.IsWhiteSpace(line[i]);

                        if (!inWord && !isWhiteSpace)
                        {
                            words++;
                        }

                        inWord = !isWhiteSpace;
                    }
                }
            }

            return words;
        }
    }
}