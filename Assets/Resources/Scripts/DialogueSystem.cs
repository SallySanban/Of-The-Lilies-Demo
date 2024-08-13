using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Characters;

namespace Dialogue
{
    public class DialogueSystem : MonoBehaviour
    {
        [SerializeField] private DialogueSystemConfig _config;
        public DialogueSystemConfig config => _config;

        [SerializeField] private GameObject _speechBubblePrefab;
        public GameObject speechBubblePrefab => _speechBubblePrefab;

        public DialogueContainer dialogueContainer = new DialogueContainer();
        public ConversationManager conversationManager { get; private set; }
        public SpeechBubbleManager speechBubbleManager { get; private set; }

        private TextArchitect textArchitect;

        public bool speechBubbleActive = false;

        [SerializeField] public Vector2 size;

        public static DialogueSystem Instance { get; private set; }

        public delegate void DialogueSystemEvent();
        public event DialogueSystemEvent onUserNext;

        protected Coroutine showingTextboxCoroutine, hidingTextboxCoroutine;

        public bool isTextboxShowing => showingTextboxCoroutine != null;
        public bool isTextboxHiding => hidingTextboxCoroutine != null;

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
            speechBubbleManager = new SpeechBubbleManager(speechBubblePrefab);
        }

        public void OnUserNext()
        {
            onUserNext?.Invoke();
        }

        public void ApplySpeakerDataToDialogueContainer(string speakerName)
        {
            Character character = CharacterManager.Instance.GetCharacter(speakerName);

            CharacterConfigData config = character.config != null ? character.config : CharacterManager.Instance.GetCharacterConfig(speakerName);

            ApplySpeakerDataToDialogueContainer(config);
        }

        public void ApplySpeakerDataToDialogueContainer(CharacterConfigData config)
        {
            dialogueContainer.SetBorderColor(config.textboxBorderColor);
            dialogueContainer.nameContainer.SetBorderColor(config.nameBorderColor);
        }

        public void ShowSpeakerName(string speakerName = "") => dialogueContainer.nameContainer.Show(speakerName);

        public void HideSpeakerName() => dialogueContainer.nameContainer.Hide();

        public Coroutine SayTextbox(List<string> lines)
        {
            Conversation conversation = new Conversation(lines);

            return conversationManager.StartConversation(conversation);
        }

        public Coroutine SaySpeechBubble((string key, string value, string parameter) action)
        {
            speechBubbleActive = true;

            List<(string key, string value, string parameter)> actions = new List<(string key, string value, string parameter)>
            {
                action
            };

            return SaySpeechBubble(actions);
        }

        public Coroutine SaySpeechBubble(List<(string key, string value, string parameter)> actions)
        {
            speechBubbleActive = true;

            return speechBubbleManager.StartSpeechBubble(actions);
        }

        [ContextMenu("Show Speech Bubble Position")]
        public void ShowPosition()
        {
            Debug.Log("SPEECH BUBBLE POSITION: " + speechBubbleManager.GetPosition());
        }

        [ContextMenu("Show Speech Bubble Size")]
        public void ShowSize()
        {
            Debug.Log("SPEECH BUBBLE SIZE: " + speechBubbleManager.GetSize());
        }

        [ContextMenu("Change Speech Bubble Size")]
        public void ChangeSize()
        {
            speechBubbleManager.ChangeSize(size);
        }

        public void Show()
        {
            dialogueContainer.root.SetActive(true);
        }

        public void Hide()
        {
            dialogueContainer.dialogueText.text = "";
            dialogueContainer.root.SetActive(false);
        }
    }
}

