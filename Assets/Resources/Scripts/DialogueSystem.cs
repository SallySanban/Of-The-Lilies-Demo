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

        public DialogueContainer dialogueContainer = new DialogueContainer();
        private ConversationManager conversationManager;
        private TextArchitect textArchitect;

        public static DialogueSystem Instance { get; private set; }

        public delegate void DialogueSystemEvent();
        public event DialogueSystemEvent onUserNext;

        protected Coroutine showingTextboxCoroutine, hidingTextboxCoroutine;

        public bool isTextboxShowing => showingTextboxCoroutine != null;
        public bool isTextboxHiding => hidingTextboxCoroutine != null;

        public bool isRunningConversation => conversationManager.isRunning;

        private float fadeSpeed = 3f;

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

        public Coroutine Say(List<string> conversation)
        {
            return conversationManager.StartConversation(conversation);
        }

        public Coroutine Show()
        {
            if (isTextboxShowing) return showingTextboxCoroutine;

            if (isTextboxHiding)
            {
                StopCoroutine(hidingTextboxCoroutine);
            }

            showingTextboxCoroutine = StartCoroutine(ShowingOrHiding(true));

            return showingTextboxCoroutine;
        }

        public Coroutine Hide()
        {
            if (isTextboxHiding) return hidingTextboxCoroutine;

            if (isTextboxShowing)
            {
                StopCoroutine(showingTextboxCoroutine);
            }

            hidingTextboxCoroutine = StartCoroutine(ShowingOrHiding(false));

            return hidingTextboxCoroutine;
        }

        public IEnumerator ShowingOrHiding(bool show)
        {
            float targetAlpha = show ? 1f : 0f;

            CanvasGroup self = dialogueContainer.root.GetComponent<CanvasGroup>();

            while (self.alpha != targetAlpha)
            {
                self.alpha = Mathf.MoveTowards(self.alpha, targetAlpha, fadeSpeed * Time.deltaTime);

                if(self.alpha == 0f)
                {
                    dialogueContainer.dialogueText.text = "";
                }

                yield return null;
            }

            showingTextboxCoroutine = null;
            hidingTextboxCoroutine = null;
        }
    }
}

