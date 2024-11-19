using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;

namespace Commands
{
    public class DatabaseExtensionChoices : CommandDatabaseExtension
    {
        private static readonly string enqueueParameter = "-e";
        private static readonly string priorityParameter = "-p";

        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("Load", new Action<string[]>(LoadNewDialogueFile));
        }

        private static void LoadNewDialogueFile(string[] data)
        {
            string filename = data[0];

            bool enqueue = false;
            bool priority = false;

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(enqueueParameter, out enqueue, defaultValue: false);
            parameters.TryGetValue(priorityParameter, out priority, defaultValue: false);

            TextAsset file = Resources.Load<TextAsset>(FilePaths.storyPath + filename);

            if(file == null)
            {
                Debug.LogError($"File {file.name} does not exist.");
                return;
            }

            List<string> lines = FileManager.ReadTextAsset(file, includeBlankLines: true);

            Conversation newConversation = new Conversation(lines);

            if (priority)
            {
                DialogueSystem.Instance.conversationManager.EnqueuePriority(newConversation);
            }
            else
            {
                if (enqueue)
                {
                    DialogueSystem.Instance.conversationManager.Enqueue(newConversation);
                }
                else
                {
                    DialogueSystem.Instance.conversationManager.StartConversation(newConversation);
                }
            }
        }
    }
}

