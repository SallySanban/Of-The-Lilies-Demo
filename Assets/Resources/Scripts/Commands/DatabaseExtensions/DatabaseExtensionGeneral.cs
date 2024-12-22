using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Dialogue;

namespace Commands
{
    public class DatabaseExtensionGeneral : CommandDatabaseExtension
    {
        private static readonly string enqueueParameter = "-e";
        private static readonly string priorityParameter = "-p";

        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("Wait", new Func<string, IEnumerator>(Wait));
            database.AddCommand("Load", new Action<string[]>(Load));
        }

        private static IEnumerator Wait(string data)
        {
            if (float.TryParse(data, out float time))
            {
                yield return new WaitForSeconds(time);
            }
        }

        private static void Load(string[] data)
        {
            string filename = data[0];

            bool enqueue = false;
            bool priority = false;

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(enqueueParameter, out enqueue, defaultValue: false);
            parameters.TryGetValue(priorityParameter, out priority, defaultValue: false);

            TextAsset file = Resources.Load<TextAsset>(FilePaths.storyPath + filename);

            if (file == null)
            {
                Debug.LogError($"File {file.name} does not exist.");
                return;
            }

            List<string> lines = FileManager.ReadTextAsset(file, includeBlankLines: true);

            Conversation newConversation = new Conversation(lines);

            if (priority) //priority: go to new dialogue now but come back after
            {
                DialogueManager.Instance.conversationManager.EnqueuePriority(newConversation);
            }
            else
            {
                if (enqueue) //enqueue: go to new dialogue after this dialogue finishes
                {
                    DialogueManager.Instance.conversationManager.Enqueue(newConversation);
                }
                else //no enqueue or priority: permanently go to new dialogue
                {
                    DialogueManager.Instance.conversationManager.StartConversation(newConversation);
                }
            }
        }
    }
}

