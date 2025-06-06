using System.Collections;
using UnityEngine;
using System.Reflection;
using System.Linq;
using System;
using Dialogue;

namespace Commands
{
    public class CommandManager
    {
        private DialogueManager dialogueManager => DialogueManager.Instance;

        private static Coroutine process = null;
        public static bool isRunningProcess => process != null;

        private CommandDatabase database;

        public CommandManager()
        {
            database = new CommandDatabase();

            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] extensionTypes = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(CommandDatabaseExtension))).ToArray();

            foreach (Type extension in extensionTypes)
            {
                MethodInfo extendMethod = extension.GetMethod("Extend");
                extendMethod.Invoke(null, new object[] { database });
            }
        }

        public Coroutine Execute(string commandName, params string[] args)
        {
            Delegate command = database.GetCommand(commandName);

            if (command == null) return null;

            return StartProcess(commandName, command, args);
        }

        private Coroutine StartProcess(string commandName, Delegate command, string[] args)
        {
            StopCurrentProcess();

            process = dialogueManager.StartCoroutine(RunningProcess(command, args));

            return process;
        }

        private void StopCurrentProcess()
        {
            if (process != null)
            {
                dialogueManager.StopCoroutine(process);
            }

            process = null;
        }

        private IEnumerator RunningProcess(Delegate command, string[] args)
        {
            yield return WaitingForProcess(command, args);

            process = null;
        }

        private IEnumerator WaitingForProcess(Delegate command, string[] args)
        {
            if (command is Action)
            {
                command.DynamicInvoke();
            }
            else if (command is Action<string>)
            {
                command.DynamicInvoke(args[0]);
            }
            else if (command is Action<string[]>)
            {
                command.DynamicInvoke((object)args);
            }
            else if (command is Func<IEnumerator>)
            {
                yield return ((Func<IEnumerator>)command)();
            }
            else if (command is Func<string, IEnumerator>)
            {
                yield return ((Func<string, IEnumerator>)command)(args[0]);
            }
            else if (command is Func<string[], IEnumerator>)
            {
                yield return ((Func<string[], IEnumerator>)command)(args);
            }
        }
    }
}

