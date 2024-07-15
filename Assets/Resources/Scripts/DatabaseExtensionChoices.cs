using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Commands
{
    public class DatabaseExtensionChoices : CommandDatabaseExtension
    {
        private static readonly string enqueueParameter = "-e";

        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("Load", new Action<string[]>(LoadNewDialogueFile));
        }

        private static void LoadNewDialogueFile(string[] data)
        {
            string filePath = string.Empty;

            bool enqueue = false;

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(enqueueParameter, out enqueue, defaultValue: false);
        }
    }
}

