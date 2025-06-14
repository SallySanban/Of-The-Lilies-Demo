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
        [SerializeField] private GameObject speechBubblePrefab;

        public SceneManager sceneManager => SceneManager.Instance;

        public ConversationManager conversationManager { get; private set; }
        public CommandManager commandManager { get; private set; }

        public delegate void DialogueManagerEvent();
        public event DialogueManagerEvent onUserNext;

        public List<DialogueContainer> textboxesOnScreen = new List<DialogueContainer>();

        public DialogueContainer currentTextbox = null;

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
            if (currentTextbox != null)
            {
                if (currentTextbox.textboxType == textboxTypeToShow)
                {
                    if(currentTextbox.textboxType != DialogueContainer.ContainerType.SpeechBubble)
                    {
                        //we don't hide the textbox if they're exactly the same textbox, only change the name
                        if (!speakerName.ToLower().Equals(currentTextbox.name.text.ToLower()))
                        {
                            currentTextbox.name.text = speakerName.ToUpper();
                        }

                        if(listOfChoices != null)
                        {
                            currentTextbox.ShowChoices(listOfChoices);
                        }

                        yield break;
                    }
                    else
                    {
                        //we want to show the next textbox (which is a speech bubble), and the previous is also speech bubble,
                        //so we hide it to make way for new speech bubble
                        if (!speakerSprite.name.ToLower().Equals(currentTextbox.SpeakerSpriteName.ToLower()))
                        {
                            yield return HideTextbox(true);
                        }
                        else //we don't hide the textbox if the same person is talking
                        {
                            if (listOfChoices != null)
                            {
                                currentTextbox.ShowChoices(listOfChoices);
                            }

                            yield break;
                        }
                    }
                }
                else
                {
                    //we want to show the next textbox, but the next textbox is not the same textbox, so we hide the previous one
                    yield return HideTextbox(true); 
                }
            }

            switch (textboxTypeToShow)
            {
                case DialogueContainer.ContainerType.MainTextbox:
                case DialogueContainer.ContainerType.LeftTextbox:
                case DialogueContainer.ContainerType.RightTextbox:
                    GameObject dialogueContainer = GetTextboxContainerPrefab(textboxTypeToShow);
                    currentTextbox = new Textbox(dialogueContainer, textboxPanel, textboxTypeToShow, speakerSprite, speakerName, listOfChoices);
                    break;
                case DialogueContainer.ContainerType.SpeechBubble:
                    currentTextbox = new SpeechBubble(speechBubblePrefab, speakerSprite, speakerName, listOfChoices);
                    break;
            }

            conversationManager.textArchitect = new TextArchitect(currentTextbox.dialogue);
        }

        public IEnumerator HideTextbox(bool immediate)
        {
            if (currentTextbox == null) yield break;

            if (immediate)
            {
                DestroyImmediate(currentTextbox.root);
            }
            else
            {
                yield return currentTextbox.Hide();
                currentTextbox.dialogue.text = "";
            }

            currentTextbox = null;
        }

        private GameObject GetTextboxContainerPrefab(DialogueContainer.ContainerType textboxUsed)
        {
            switch (textboxUsed)
            {
                case DialogueContainer.ContainerType.MainTextbox:
                    return mainTextboxPrefab;
                case DialogueContainer.ContainerType.LeftTextbox:
                    return leftTextboxPrefab;
                case DialogueContainer.ContainerType.RightTextbox:
                    return rightTextboxPrefab;
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
                case -1:
                    return DialogueContainer.ContainerType.SpeechBubble;
                default:
                    return DialogueContainer.ContainerType.MainTextbox;
            }
        }
    }
}