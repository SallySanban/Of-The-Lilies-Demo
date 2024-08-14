using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Dialogue
{
    public class SpeechBubbleManager
    {
        public DialogueSystem dialogueSystem => DialogueSystem.Instance;

        private Coroutine process = null;
        public bool isRunning => process != null;

        private GameObject speechBubblePrefab = null;
        GameObject currentSpeechBubble = null;
        private TextMeshProUGUI speechBubbleText = null;
        private RectTransform speechBubbleTransform = null;
        private CanvasGroup speechBubbleCanvasGroup = null;

        private TextArchitect textArchitect;
        private bool userNext = false;

        private float minWidth = 300f;
        private float minHeight = 250f;
        private float maxWidth = 680f;
        private float maxHeight = 300f;

        protected Coroutine showingBubbleCoroutine, hidingBubbleCoroutine;

        public bool isBubbleShowing => showingBubbleCoroutine != null;
        public bool isBubbleHiding => hidingBubbleCoroutine != null;

        private float fadeSpeed = 3f;

        public SpeechBubbleManager(GameObject prefab)
        {
            if (prefab != null)
            {
                speechBubblePrefab = prefab;
                dialogueSystem.onUserNext += OnUserNext;
            }
        }

        public void ChangeSize(Vector2 size)
        {
            speechBubbleTransform.sizeDelta = size;
        }

        private void OnUserNext()
        {
            if (dialogueSystem.speechBubbleActive)
            {
                userNext = true;
            }
        }

        public Coroutine StartSpeechBubble(List<(string command, string key, string value)> actions)
        {
            StopSpeechBubble();

            process = dialogueSystem.StartCoroutine(RunningSpeechBubble(actions));

            return process;
        }

        public void StopSpeechBubble()
        {
            if (!isRunning)
            {
                return;
            }

            dialogueSystem.StopCoroutine(process);
            process = null;
        }

        private IEnumerator RunningSpeechBubble(List<(string command, string key, string value)> actions)
        {
            for (int i = 0; i < actions.Count; i++)
            {
                var action = actions[i];

                if(action.command == "Command")
                {
                    Debug.Log(action.key);

                    switch (action.key)
                    {
                        case "Enable Interactable":
                            InteractableManager.Instance.EnableDisableInteractable(action.value, true);
                            break;
                        case "Disable Interactable":
                            InteractableManager.Instance.EnableDisableInteractable(action.value, false);
                            break;
                    }    
                }
                else if(action.command == "Speech Bubble")
                {
                    CreateSpeechBubble(action.key);
                    Show();
                    yield return TalkSpeechBubble(action.value);

                    if(!SceneManager.Instance.inVNMode) yield return WaitForUserInput();
                }

                if (!SceneManager.Instance.inVNMode)
                {
                    if (i == actions.Count - 1)
                    {
                        Hide();
                    }
                    else
                    {
                        Hide(true);
                    }
                }
            }

            Debug.Log("Actions ended");
            dialogueSystem.speechBubbleActive = false;
            process = null;
        }

        public Vector3 GetPosition()
        {
            return currentSpeechBubble.transform.position;
        }

        public Vector3 GetSize()
        {
            return speechBubbleTransform.sizeDelta;
        }

        private void CreateSpeechBubble(string characterName)
        {
            Transform sprite = GameObject.Find(characterName).GetComponentInChildren<Transform>();

            Vector3 spriteOffset;
            if(characterName == "Ahlai")
            {
                spriteOffset = new Vector3(0.53f, 1.93f, 0f);
            }
            else
            {
                spriteOffset = new Vector3(0.29f, 1.48f, 0f);
            }
            
            Vector3 speechBubblePosition = sprite.position + spriteOffset;

            currentSpeechBubble = Object.Instantiate(speechBubblePrefab, speechBubblePosition, Quaternion.identity, sprite.parent);
            speechBubbleText = currentSpeechBubble.GetComponentInChildren<TextMeshProUGUI>();
            speechBubbleTransform = currentSpeechBubble.GetComponent<RectTransform>();
            speechBubbleCanvasGroup = currentSpeechBubble.GetComponent<CanvasGroup>();

            textArchitect = new TextArchitect(speechBubbleText);

            speechBubbleCanvasGroup.alpha = 0f;
        }

        private IEnumerator TalkSpeechBubble(string dialogue)
        {
            Debug.Log(dialogue);
            speechBubbleText.text = dialogue;

            float newWidth = Mathf.Clamp(speechBubbleText.preferredWidth, minWidth, maxWidth);
            
            speechBubbleTransform.sizeDelta = new Vector2(newWidth, speechBubbleTransform.sizeDelta[1]);

            speechBubbleText.ForceMeshUpdate();

            float newHeight = Mathf.Clamp(speechBubbleText.preferredHeight * speechBubbleText.textInfo.lineCount, minHeight, maxHeight);

            speechBubbleTransform.sizeDelta = new Vector2(newWidth, newHeight);

            if (speechBubbleText.textInfo.lineCount == 1)
            {
                speechBubbleText.alignment = TextAlignmentOptions.Midline;
            }
            else if(speechBubbleText.textInfo.lineCount == 2)
            {
                speechBubbleText.alignment = TextAlignmentOptions.Top;
            }
            else
            {
                Debug.LogError("Text is too long");
            }

            speechBubbleText.text = "";
            dialogue = TagManager.Inject(dialogue);
            textArchitect.Build(dialogue);

            while (textArchitect.isBuilding)
            {
                if (userNext)
                {
                    if (textArchitect.isBuilding)
                    {
                        textArchitect.ForceComplete();
                    }

                    userNext = false;
                }

                yield return null;
            }
        }

        public IEnumerator WaitForUserInput()
        {
            while (!userNext)
            {
                yield return null;
            }

            userNext = false;
        }

        private Coroutine Show(bool immediate = false)
        {
            if (isBubbleShowing) return showingBubbleCoroutine;

            if (isBubbleHiding)
            {
                dialogueSystem.StopCoroutine(hidingBubbleCoroutine);
            }

            showingBubbleCoroutine = dialogueSystem.StartCoroutine(ShowingOrHiding(true, immediate));

            return showingBubbleCoroutine;
        }

        public Coroutine Hide(bool immediate = false)
        {
            if (isBubbleHiding) return hidingBubbleCoroutine;

            if (isBubbleShowing)
            {
                dialogueSystem.StopCoroutine(showingBubbleCoroutine);
            }

            hidingBubbleCoroutine = dialogueSystem.StartCoroutine(ShowingOrHiding(false, immediate));

            return hidingBubbleCoroutine;
        }

        private IEnumerator ShowingOrHiding(bool show, bool immediate)
        {
            float targetAlpha = show ? 1f : 0f;

            CanvasGroup self = speechBubbleCanvasGroup;

            if (immediate)
            {
                self.alpha = targetAlpha;
                Object.Destroy(self.gameObject);
            }
            else
            {
                while (self.alpha != targetAlpha)
                {
                    self.alpha = Mathf.MoveTowards(self.alpha, targetAlpha, fadeSpeed * Time.deltaTime);

                    if (self.alpha == 0f)
                    {
                        Object.Destroy(self.gameObject);
                        break;
                    }

                    yield return null;
                }
            }

            showingBubbleCoroutine = null;
            hidingBubbleCoroutine = null;
        }
    }
}
