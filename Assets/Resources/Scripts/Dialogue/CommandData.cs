using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Dialogue
{
    public class CommandData
    {
        public List<Command> commands;

        private const char commandDelimiter = ',';
        private const char argumentsId = '(';
        private const char argumentsDelimiter = ' ';
        private const string waitCommandId = "[wait]";
        private const string waitUserInputCommandId = "[input]";

        public struct Command
        {
            public string name;
            public string[] arguments;
            public bool waitForCompletion;
            public bool waitForUserInput;
        }

        public CommandData(string rawCommands)
        {
            commands = RipCommands(rawCommands);
        }

        private List<Command> RipCommands(string rawCommands)
        {
            string[] data = rawCommands.Split(commandDelimiter, System.StringSplitOptions.RemoveEmptyEntries);

            List<Command> commandsList = new List<Command>();

            foreach (string cmd in data)
            {
                Command command = new Command();

                int index = cmd.IndexOf(argumentsId);
                command.name = cmd.Substring(0, index).Trim();

                if (command.name.ToLower().StartsWith(waitCommandId))
                {
                    command.name = command.name.Substring(waitCommandId.Length);
                    command.waitForCompletion = true;
                }
                else if(command.name.ToLower().StartsWith(waitUserInputCommandId))
                {
                    command.name = command.name.Substring(waitUserInputCommandId.Length);
                    command.waitForUserInput = true;
                }
                else
                {
                    command.waitForCompletion = false;
                    command.waitForUserInput = false;
                }

                command.arguments = GetArgs(cmd.Substring(index + 1, cmd.Length - index - 2));
                commandsList.Add(command);
            }

            return commandsList;
        }

        private string[] GetArgs(string args)
        {
            List<string> argumentsList = new List<string>();

            StringBuilder currentArg = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == '"')
                {
                    inQuotes = !inQuotes;
                    continue;
                }

                if (!inQuotes && args[i] == argumentsDelimiter)
                {
                    argumentsList.Add(currentArg.ToString());
                    currentArg.Clear();
                    continue;
                }

                currentArg.Append(args[i]);
            }

            if (currentArg.Length > 0)
            {
                argumentsList.Add(currentArg.ToString());
            }

            return argumentsList.ToArray();
        }
    }
}

