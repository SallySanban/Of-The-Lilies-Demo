using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;

namespace Characters
{
    public abstract class Character
    {
        public string name = "";
        public string displayName = "";
        public RectTransform root = null;
        public CharacterConfigData config;

        public Animator animator;

        protected CharacterManager manager => CharacterManager.Instance;

        public DialogueSystem dialogueSystem => DialogueSystem.Instance;

        //Coroutines
        protected Coroutine showingCharacterCoroutine, hidingCharacterCoroutine;
        protected Coroutine movingCharacterCoroutine;

        public bool isCharacterShowing => showingCharacterCoroutine != null;
        public bool isCharacterHiding => hidingCharacterCoroutine != null;
        public bool isCharacterMoving => movingCharacterCoroutine != null;
        public virtual bool isCharacterVisible { get; set; }

        public Character(string name, CharacterConfigData config, GameObject prefab)
        {
            this.name = name;
            displayName = name;
            this.config = config;

            if(prefab != null)
            {
                GameObject ob = Object.Instantiate(prefab, manager.characterPanel);
                ob.name = name;

                ob.SetActive(true);

                root = ob.GetComponent<RectTransform>();

                animator = root.GetComponentInChildren<Animator>();
            }
        }

        public Coroutine Say(string dialogue) => Say(new List<string> { dialogue });

        public Coroutine Say(List<string> dialogue)
        {
            dialogueSystem.ShowSpeakerName(displayName);
            dialogueSystem.ApplySpeakerDataToDialogueContainer(name);

            return dialogueSystem.Say(dialogue);
        }

        public virtual Coroutine Show()
        {
            if (isCharacterShowing) return showingCharacterCoroutine;

            if (isCharacterHiding)
            {
                manager.StopCoroutine(hidingCharacterCoroutine);
            }

            showingCharacterCoroutine = manager.StartCoroutine(ShowingOrHiding(true));

            return showingCharacterCoroutine;
        }

        public virtual Coroutine Hide()
        {
            if(isCharacterHiding) return hidingCharacterCoroutine;

            if (isCharacterShowing)
            {
                manager.StopCoroutine(showingCharacterCoroutine);
            }

            hidingCharacterCoroutine = manager.StartCoroutine(ShowingOrHiding(false));

            return hidingCharacterCoroutine;
        }

        public virtual IEnumerator ShowingOrHiding(bool show)
        {
            Debug.Log("Show and hide cannot be called from a base character type");
            yield return null;
        }

        public virtual void SetPosition(Vector2 position)
        {
            if (root == null) return;

            (Vector2 minAnchorTarget, Vector2 maxAnchorTarget) = ConvertUIPositionToAnchorPosition(position);

            root.anchorMin = minAnchorTarget;
            root.anchorMax = maxAnchorTarget;
        }

        public virtual Coroutine MoveToPosition(Vector2 position, float speed = 4f, bool smooth = true)
        {
            if (root == null) return null;

            if (isCharacterMoving)
            {
                manager.StopCoroutine(movingCharacterCoroutine);
            }

            movingCharacterCoroutine = manager.StartCoroutine(MovingToPosition(position, speed, smooth));

            return movingCharacterCoroutine;
        }

        private IEnumerator MovingToPosition(Vector2 position, float speed, bool smooth)
        {
            (Vector2 minAnchorTarget, Vector2 maxAnchorTarget) = ConvertUIPositionToAnchorPosition(position);

            Vector2 padding = root.anchorMax - root.anchorMin;

            while(root.anchorMin != minAnchorTarget || root.anchorMax != maxAnchorTarget)
            {
                root.anchorMin = smooth ?
                    Vector2.Lerp(root.anchorMin, minAnchorTarget, speed * Time.deltaTime) :
                    Vector2.MoveTowards(root.anchorMin, minAnchorTarget, speed * Time.deltaTime * 0.35f);

                root.anchorMax = root.anchorMin + padding;

                if (smooth && Vector2.Distance(root.anchorMin, minAnchorTarget) <= 0.0001f)
                {
                    root.anchorMin = minAnchorTarget;
                    root.anchorMax = maxAnchorTarget;

                    break;
                }

                yield return null;
            }

            movingCharacterCoroutine = null;
        }

        protected (Vector2, Vector2) ConvertUIPositionToAnchorPosition(Vector2 position)
        {
            Vector2 padding = root.anchorMax - root.anchorMin;

            float maxX = 1f - padding.x;
            float maxY = 1f - padding.y;

            Vector2 minAnchorTarget = new Vector2(maxX * position.x, maxY * position.y);
            Vector2 maxAnchorTarget = minAnchorTarget + padding;

            return (minAnchorTarget, maxAnchorTarget);
        }

        public virtual void OnReceiveCastingExpression(int layer, string expression)
        {

        }

        public enum CharacterType
        {
            Text,
            Sprite
        }
    }
}
