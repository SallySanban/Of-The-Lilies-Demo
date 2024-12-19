using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commands;

namespace Dialogue
{
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance { get; private set; }

        [SerializeField] private RectTransform textboxPanel;
        [SerializeField] private GameObject mainTextboxPrefab;
        [SerializeField] private GameObject leftTextboxPrefab;
        [SerializeField] private GameObject rightTextboxPrefab;

        public SceneManager sceneManager => SceneManager.Instance;

        public ConversationManager conversationManager { get; private set; }
        public CommandManager commandManager { get; private set; }

        private const float ANIMATION_DECREASE_TIME = 0.2f; //time that the currentTextbox animation is decreased by before the text architect starts

        public delegate void DialogueManagerEvent();
        public event DialogueManagerEvent onUserNext;

        public List<DialogueContainer> textboxesOnScreen = new List<DialogueContainer>();

        public DialogueContainer currentTextbox = null;

        //public bool isRunningConversation => conversationManager.isRunning;

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

            conversationManager = new ConversationManager();
            commandManager = new CommandManager();
        }

        public void OnUserNext()
        {
            onUserNext?.Invoke();
        }

        public Coroutine Say(List<string> lines, string filePath = "")
        {
            Conversation conversation = new Conversation(lines);

            return conversationManager.StartConversation(conversation);
        }

        public IEnumerator ShowTextbox(DialogueContainer.ContainerType textboxTypeToShow, string speakerName = "", GameObject speakerSprite = null, string[] listOfChoices = null)
        {
            if(currentTextbox != null)
            {
                if (currentTextbox.textboxType == textboxTypeToShow)
                {
                    if(currentTextbox.name.text.ToLower().Equals(speakerName.ToLower()))
                    {
                        yield break;
                    }
                }

                switch (currentTextbox.textboxType)
                {
                    case DialogueContainer.ContainerType.SpeechBubble:
                        yield return HideTextbox(true);

                        break;
                    case DialogueContainer.ContainerType.LeftTextbox:
                    case DialogueContainer.ContainerType.RightTextbox:
                        StartCoroutine(AnimatePreviousTextboxes());
                        break;
                }
            }

            GameObject dialogueContainer = GetDialogueContainerPrefab(textboxTypeToShow);
            
            switch (textboxTypeToShow)
            {
                case DialogueContainer.ContainerType.Textbox:
                    currentTextbox = new Textbox(dialogueContainer, textboxPanel, textboxTypeToShow, speakerName, listOfChoices);
                    break;
                case DialogueContainer.ContainerType.LeftTextbox:
                case DialogueContainer.ContainerType.RightTextbox:
                    currentTextbox = new Textbox(dialogueContainer, textboxPanel, textboxTypeToShow, speakerName, listOfChoices);

                    Animator currentTextboxAnimator = currentTextbox.root.GetComponent<Animator>();

                    float animationTime = currentTextboxAnimator.GetCurrentAnimatorStateInfo(0).length;
                    yield return new WaitForSeconds(animationTime - ANIMATION_DECREASE_TIME);

                    textboxesOnScreen.Add(currentTextbox);

                    break;
                case DialogueContainer.ContainerType.SpeechBubble:
                    currentTextbox = new SpeechBubble(speakerSprite, speakerName, listOfChoices);

                    break;
            }

            conversationManager.textArchitect = new TextArchitect(currentTextbox.dialogue);
        }

        public IEnumerator HideTextbox(bool immediate)
        {
            if (currentTextbox == null) yield break;

            switch (currentTextbox.textboxType)
            {
                case DialogueContainer.ContainerType.Textbox:
                    if (immediate)
                    {
                        DestroyImmediate(currentTextbox.root);
                    }
                    else
                    {
                        yield return currentTextbox.Hide();
                    }

                    currentTextbox = null;

                    break;
                case DialogueContainer.ContainerType.SpeechBubble:
                    if (immediate)
                    {
                        currentTextbox.root.SetActive(false);
                    }
                    else
                    {
                        currentTextbox.Hide();
                    }

                    currentTextbox = null;

                    break;
                case DialogueContainer.ContainerType.LeftTextbox:
                case DialogueContainer.ContainerType.RightTextbox:
                    foreach(DialogueContainer dialogueContainer in textboxesOnScreen)
                    {
                        DestroyImmediate(dialogueContainer.root);
                        currentTextbox = null;
                    }

                    break;
                default:
                    break;
            }
        }

        private IEnumerator AnimatePreviousTextboxes()
        {
            if (textboxesOnScreen.Count == 1)
            {
                Animator moveUpTextboxAnimator = textboxesOnScreen[0].root.GetComponent<Animator>();

                moveUpTextboxAnimator.SetTrigger("NextLine");

                float animationTime = moveUpTextboxAnimator.GetCurrentAnimatorStateInfo(0).length;
                yield return new WaitForSeconds(animationTime);
            }
            else if (textboxesOnScreen.Count == 2)
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

        private GameObject GetDialogueContainerPrefab(DialogueContainer.ContainerType textboxUsed)
        {
            switch (textboxUsed)
            {
                case DialogueContainer.ContainerType.Textbox:
                    return mainTextboxPrefab;
                case DialogueContainer.ContainerType.RightTextbox:
                    return rightTextboxPrefab;
                case DialogueContainer.ContainerType.LeftTextbox:
                    return leftTextboxPrefab;
                case DialogueContainer.ContainerType.SpeechBubble:
                    return null;
                default:
                    return null;
            }
        }

        public DialogueContainer.ContainerType GetTextboxTypeFromPosition(float position)
        {
            switch (position)
            {
                case 0:
                    return DialogueContainer.ContainerType.LeftTextbox;
                case 1:
                    return DialogueContainer.ContainerType.RightTextbox;
                default:
                    return DialogueContainer.ContainerType.Textbox;
            }
        }
    }
}