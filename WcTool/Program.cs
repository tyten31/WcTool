using System.CommandLine;
using System.CommandLine.NamingConventionBinder;

namespace WcTool
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var commandManager = new CommandManager();
            var exit = false;

            ShowHelp();

            while (!exit)
            {
                Console.Write(">");
                var input = Console.ReadLine();

                if (!string.IsNullOrEmpty(input))
                {
                    exit = input.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase);

                    if (!exit)
                    {
                        var command = commandManager.BuildCommand(input.Trim().Split('|', StringSplitOptions.RemoveEmptyEntries));

                        // Check if the command is valid
                        if (commandManager.IsValidCommand(command))
                        {
                            var rootCommand = commandManager.BuildRootCommand(command);
                            rootCommand.Handler = CommandHandler.Create<string, string>(commandManager.ExecuteCommand);
                            rootCommand.Invoke(commandManager.BuildInvokeCommand(command));
                        }
                        else
                        {
                            Console.WriteLine("Invalid Command");
                            Console.WriteLine("");

                            ShowHelp();
                        }

                        Console.WriteLine("");
                    }
                }
            }
        }

        private static void ShowHelp()
        {
            Console.WriteLine("Usage: ccwc -<command> <file name>");
            Console.WriteLine("Commands:");
            Console.WriteLine(" -c <Output # of bytes in file>");
            Console.WriteLine(" -l <Output # of lines in file>");
            Console.WriteLine(" -w <Output # of words in file>");
            Console.WriteLine(" -m <Output # of characters in file>");
            Console.WriteLine("");
        }
    }
}