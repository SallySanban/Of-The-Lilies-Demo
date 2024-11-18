using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
//using Characters;
using UnityEngine.TextCore.Text;
using UnityEditor;

namespace Dialogue
{
    public class DialogueSystem : MonoBehaviour
    {
        //[SerializeField] private DialogueSystemConfig _config;
        //public DialogueSystemConfig config => _config;

        private ContainerToUse currentMode = ContainerToUse.Textbox;

        //[SerializeField] private GameObject _speechBubblePrefab;
        //public GameObject speechBubblePrefab => _speechBubblePrefab;

        //public DialogueContainer dialogueContainer = new DialogueContainer();

        public Transform textboxContainer;

        //public DialogueContainer textbox;
        public GameObject leftTextbox;
        public GameObject rightTextbox;

        public ConversationManager conversationManager { get; private set; }
        //public SpeechBubbleManager speechBubbleManager { get; private set; }

        private TextArchitect textArchitect;

        //public bool speechBubbleActive = false;

        [SerializeField] public Vector2 size;

        public static DialogueSystem Instance { get; private set; }

        public delegate void DialogueSystemEvent();
        public event DialogueSystemEvent onUserNext;

        private List<DialogueContainer> textboxesOnScreen = new List<DialogueContainer>();

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

            //textArchitect = new TextArchitect(textbox.dialogueText);

            conversationManager = new ConversationManager();
            //speechBubbleManager = new SpeechBubbleManager(speechBubblePrefab);
        }

        public void OnUserNext()
        {
            onUserNext?.Invoke();
        }

        //public void ApplySpeakerDataToDialogueContainer(string speakerName)
        //{
        //    Character character = CharacterManager.Instance.GetCharacter(speakerName);

        //    CharacterConfigData config = character.config != null ? character.config : CharacterManager.Instance.GetCharacterConfig(speakerName);

        //    ApplySpeakerDataToDialogueContainer(config);
        //}

        //public void ApplySpeakerDataToDialogueContainer(CharacterConfigData config)
        //{
        //    dialogueContainer.SetBorderColor(config.textboxBorderColor);
        //    dialogueContainer.nameContainer.SetBorderColor(config.nameBorderColor);
        //}

        //public void ShowSpeakerName(string speakerName = "") => dialogueContainer.nameContainer.Show(speakerName);

        //public void HideSpeakerName() => dialogueContainer.nameContainer.Hide();

        public Coroutine Say(List<string> lines, string filePath = "")
        {
            Conversation conversation = new Conversation(lines);

            return conversationManager.StartConversation(conversation);
        }

        //public Coroutine SaySpeechBubble((string key, string value, string parameter) action)
        //{
        //    speechBubbleActive = true;

        //    List<(string key, string value, string parameter)> actions = new List<(string key, string value, string parameter)>
        //    {
        //        action
        //    };

        //    return SaySpeechBubble(actions);
        //}

        //public Coroutine SaySpeechBubble(List<(string key, string value, string parameter)> actions)
        //{
        //    speechBubbleActive = true;

        //    return speechBubbleManager.StartSpeechBubble(actions);
        //}

        //[ContextMenu("Show Speech Bubble Position")]
        //public void ShowPosition()
        //{
        //    Debug.Log("SPEECH BUBBLE POSITION: " + speechBubbleManager.GetPosition());
        //}

        //[ContextMenu("Show Speech Bubble Size")]
        //public void ShowSize()
        //{
        //    Debug.Log("SPEECH BUBBLE SIZE: " + speechBubbleManager.GetSize());
        //}

        //[ContextMenu("Change Speech Bubble Size")]
        //public void ChangeSize()
        //{
        //    speechBubbleManager.ChangeSize(size);
        //}

        public TextArchitect ShowTextbox(ContainerToUse textboxToShow)
        {
            DialogueContainer textbox = null;

            switch (textboxToShow)
            {
                case ContainerToUse.Textbox:
                    //textbox.root.SetActive(true);
                    break;
                case ContainerToUse.LeftTextbox:
                    StartCoroutine(AnimatePreviousTextboxes());
                    textbox = new DialogueContainer(leftTextbox, textboxContainer);
                    break;
                case ContainerToUse.RightTextbox:
                    StartCoroutine(AnimatePreviousTextboxes());
                    textbox = new DialogueContainer(rightTextbox, textboxContainer);
                    break;
            }

            textArchitect = new TextArchitect(textbox.dialogue);

            textboxesOnScreen.Add(textbox);

            return textArchitect;
        }

        private IEnumerator AnimatePreviousTextboxes()
        {
            if (textboxesOnScreen.Count == 1)
            {
                Animator moveUpTextboxAnimator = textboxesOnScreen[0].root.GetComponent<Animator>();

                moveUpTextboxAnimator.SetTrigger("NextLine");

                yield return null;
            }
            else if(textboxesOnScreen.Count == 2)
            {
                Animator disappearingTextboxAnimator = textboxesOnScreen[0].root.GetComponent<Animator>();
                Animator moveUpTextboxAnimator = textboxesOnScreen[1].root.GetComponent<Animator>();

                moveUpTextboxAnimator.SetTrigger("NextLine");
                disappearingTextboxAnimator.SetTrigger("Disappear");

                float animationTime = disappearingTextboxAnimator.GetCurrentAnimatorStateInfo(0).length;
                yield return new WaitForSeconds(animationTime);

                DestroyImmediate(textboxesOnScreen[0].root);
                textboxesOnScreen.RemoveAt(0);
            }
        }

        //public void Hide()
        //{
        //    dialogueContainer.dialogueText.text = "";
        //    dialogueContainer.root.SetActive(false);
        //}

        public enum ContainerToUse
        {
            Textbox,
            LeftTextbox,
            RightTextbox,
            SpeechBubble
        }
    }
}