using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;

namespace Commands
{
    public class DatabaseExtensionTextbox : CommandDatabaseExtension
    {
        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("ShowTextbox", new Func<IEnumerator>(Show));
            database.AddCommand("HideTextbox", new Func<IEnumerator>(Hide));
        }

        private static IEnumerator Show()
        {
            DialogueSystem.Instance.Show();

            while (DialogueSystem.Instance.isTextboxShowing)
            {
                yield return null;
            }
        }

        private static IEnumerator Hide()
        {
            DialogueSystem.Instance.Hide();

            while (DialogueSystem.Instance.isTextboxHiding)
            {
                yield return null;
            }
        }
    }
}

