using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using Commands;

namespace Testing
{
    public class DatabaseExtensionExample : CommandDatabaseExtension
    {
        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("Print", new Action(PrintDefaultMessage));
            database.AddCommand("Print_1P", new Action<string>(PrintUserMessage));
            database.AddCommand("Print_MP", new Action<string[]>(PrintLines));

            database.AddCommand("lambda", new Action(() => { Debug.Log("Printing a default message to console."); }));
            database.AddCommand("lambda_1P", new Action<string>((arg) => { Debug.Log($"Message: {arg}"); }));
            database.AddCommand("lambda_MP", new Action<string[]>((args) => { Debug.Log(string.Join(", ", args)); }));

            database.AddCommand("Process", new Func<IEnumerator>(SimpleProcess));
            database.AddCommand("Process_1P", new Func<string, IEnumerator>(LineProcess));
            database.AddCommand("Process_MP", new Func<string[], IEnumerator>(MultilineProcess));
        }

        private static void PrintDefaultMessage()
        {
            Debug.Log("HIIIII Printing default message to console!");
        }

        private static void PrintUserMessage(string message)
        {
            Debug.Log($"User message: {message}");
        }

        private static void PrintLines(string[] lines)
        {
            foreach (string line in lines)
            {
                Debug.Log($"User message: {line}");
            }
        }

        private static IEnumerator SimpleProcess()
        {
            for (int i = 1; i <= 5; i++)
            {
                Debug.Log($"Process running for {i} seconds");
                yield return new WaitForSeconds(1);
            }
        }

        private static IEnumerator LineProcess(string data)
        {
            if (int.TryParse(data, out int num))
            {
                for (int i = 1; i <= num; i++)
                {
                    Debug.Log($"Process running for {i} seconds");
                    yield return new WaitForSeconds(1);
                }
            }
        }

        private static IEnumerator MultilineProcess(string[] data)
        {
            foreach (string line in data)
            {
                Debug.Log($"LINE: {line}");
                yield return new WaitForSeconds(1);
            }
        }
    }
}

