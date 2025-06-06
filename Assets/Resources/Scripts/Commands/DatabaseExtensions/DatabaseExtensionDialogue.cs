using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;

namespace Commands
{
    public class DatabaseExtensionDialogue : CommandDatabaseExtension
    {
        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("HideTextbox", new Func<string, IEnumerator>(HideTextbox));
            database.AddCommand("Proceed", new Action<string>(Proceed));
        }

        private static IEnumerator HideTextbox(string data)
        {
            if (bool.TryParse(data, out bool immediate))
            {
                yield return DialogueManager.Instance.HideTextbox(immediate);
            }
        }

        private static void Proceed(string data)
        {
            if (bool.TryParse(data, out bool proceed))
            {
                DialogueManager.Instance.conversationManager.proceed = proceed;
            }
        }
    }
}

