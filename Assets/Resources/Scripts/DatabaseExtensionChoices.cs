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

        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("Load", new Action<string[]>(LoadNewDialogueFile));
        }

        private static void LoadNewDialogueFile(string[] data)
        {
            string filename = data[0];

            bool enqueue = false;

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(enqueueParameter, out enqueue, defaultValue: false);

            TextAsset file = Resources.Load<TextAsset>(FilePaths.storyFiles + filename);

            if(file == null)
            {
                Debug.LogError($"File {file.name} does not exist.");
                return;
            }

            List<string> lines = FileManager.ReadTextAsset(file, includeBlankLines: true);

            Conversation newConversation = new Conversation(lines);

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

