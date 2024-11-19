using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;

namespace Dialogue
{
    public class DialogueSystem : MonoBehaviour
    {
        public Transform textboxContainer;

        public GameObject mainTextbox;
        public GameObject leftTextbox;
        public GameObject rightTextbox;

        public ConversationManager conversationManager { get; private set; }

        private TextArchitect textArchitect;

        public static DialogueSystem Instance { get; private set; }

        public delegate void DialogueSystemEvent();
        public event DialogueSystemEvent onUserNext;

        private List<DialogueContainer> textboxesOnScreen = new List<DialogueContainer>();

        private DialogueContainer currentTextbox = null;

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

        public TextArchitect ShowTextbox(ContainerToUse textboxToShow, string speakerName)
        {
            if(currentTextbox != null && currentTextbox.textboxType == ContainerToUse.Textbox)
            {
                Object.DestroyImmediate(currentTextbox.root);
                currentTextbox = null;
            }

            GameObject dialogueContainer = GetDialogueContainerToUse(textboxToShow);

            if(textboxToShow != ContainerToUse.Textbox) StartCoroutine(AnimatePreviousTextboxes());

            currentTextbox = new DialogueContainer(dialogueContainer, textboxContainer, textboxToShow, speakerName);

            textArchitect = new TextArchitect(currentTextbox.dialogue);

            if (textboxToShow != ContainerToUse.Textbox) textboxesOnScreen.Add(currentTextbox);

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

        private GameObject GetDialogueContainerToUse(ContainerToUse textboxUsed)
        {
            switch(textboxUsed)
            {
                case ContainerToUse.Textbox:
                    return mainTextbox;
                case ContainerToUse.RightTextbox:
                    return rightTextbox;
                case ContainerToUse.LeftTextbox:
                    return leftTextbox;
                default:
                    return mainTextbox;
            }
        }

        public enum ContainerToUse
        {
            Textbox,
            LeftTextbox,
            RightTextbox,
            SpeechBubble
        }
    }
}