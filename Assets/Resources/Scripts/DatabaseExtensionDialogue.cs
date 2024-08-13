using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;

namespace Commands
{
    public class DatabaseExtensionDialogue : CommandDatabaseExtension
    {
        private static string dialogueParameter => "-dialogue";

        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("ShowTextbox", new Func<IEnumerator>(ShowTextbox));
            database.AddCommand("HideTextbox", new Func<IEnumerator>(HideTextbox));
            database.AddCommand("ShowSpeechBubble", new Func<string[], IEnumerator>(ShowSpeechBubble));
            database.AddCommand("HideSpeechBubble", new Func<string, IEnumerator>(HideSpeechBubble));
        }

        private static IEnumerator ShowTextbox()
        {
            DialogueSystem.Instance.Show();

            while (DialogueSystem.Instance.isTextboxShowing)
            {
                yield return null;
            }
        }

        private static IEnumerator HideTextbox()
        {
            DialogueSystem.Instance.Hide();

            while (DialogueSystem.Instance.isTextboxHiding)
            {
                yield return null;
            }
        }

        private static IEnumerator ShowSpeechBubble(string[] data)
        {
            SceneManager.Instance.inVNMode = true;

            string characterName = data[0];

            var parameters = ConvertDataToParameters(data);

            string dialogue = "";

            parameters.TryGetValue(dialogueParameter, out dialogue);

            var action = ("Speech Bubble", characterName, dialogue);

            yield return DialogueSystem.Instance.SaySpeechBubble(action);

            while (DialogueSystem.Instance.speechBubbleActive)
            {
                yield return null;
            }

            //doesn't end VN here because there must be hide speech bubble after
        }

        private static IEnumerator HideSpeechBubble(string data)
        {
            if (bool.TryParse(data, out bool immediate))
            {
                DialogueSystem.Instance.speechBubbleManager.Hide(immediate);

                while (DialogueSystem.Instance.speechBubbleManager.isBubbleHiding)
                {
                    yield return null;
                }
            }

            SceneManager.Instance.inVNMode = false;
        }
    }
}

