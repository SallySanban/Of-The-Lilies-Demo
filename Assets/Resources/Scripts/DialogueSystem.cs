using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Dialogue
{
    public class DialogueSystem : MonoBehaviour
    {
        public DialogueContainer dialogueContainer = new DialogueContainer();
        private ConversationManager conversationManager;
        private TextArchitect textArchitect;

        public static DialogueSystem Instance { get; private set; }

        public delegate void DialogueSystemEvent();
        public event DialogueSystemEvent onUserNext;

        public bool isRunningConversation => conversationManager.isRunning;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;

                Initialize();
            }
            else
            {
                DestroyImmediate(gameObject);
            }
        }

        bool initialized = false;

        private void Initialize()
        {
            if (initialized) return;

            textArchitect = new TextArchitect(dialogueContainer.dialogueText);
            conversationManager = new ConversationManager(textArchitect);
        }

        public void OnUserNext()
        {
            onUserNext?.Invoke();
        }

        public void ShowSpeakerName(string speakerName = "") => dialogueContainer.nameContainer.Show(speakerName);

        public void HideSpeakerName() => dialogueContainer.nameContainer.Hide();

        public void Say(string speaker, string dialogue)
        {
            List<string> conversation = new List<string>() { $"{speaker} \"{dialogue}\"" };
            Say(conversation);
        }

        public void Say(List<string> conversation)
        {
            conversationManager.StartConversation(conversation);
        }
    }
}

