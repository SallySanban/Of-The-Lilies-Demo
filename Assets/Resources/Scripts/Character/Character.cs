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

        public Vector2 startingLeftPosition = new Vector2(-0.5f, 0);
        public Vector2 startingRightPosition = new Vector2(1.5f, 0);

        protected CharacterManager manager => CharacterManager.Instance;

        public DialogueSystem dialogueSystem => DialogueSystem.Instance;

        protected Coroutine movingCharacterCoroutine;

        public bool isCharacterMoving => movingCharacterCoroutine != null;
        public virtual bool isCharacterVisible { get; set; }

        public Character(string name, CharacterConfigData config, GameObject prefab)
        {
            this.name = name;
            displayName = name;
            this.config = config;

            if (prefab != null)
            {
                GameObject ob = Object.Instantiate(prefab, manager.characterPanel);
                ob.name = name;

                ob.SetActive(true);

                root = ob.GetComponent<RectTransform>();

                animator = root.GetComponentInChildren<Animator>();
            }
        }

        public virtual void SetPosition(Vector2 position)
        {
            if (root == null) return;

            (Vector2 minAnchorTarget, Vector2 maxAnchorTarget) = ConvertUIPositionToAnchorPosition(position);

            root.anchorMin = minAnchorTarget;
            root.anchorMax = maxAnchorTarget;
        }

        public virtual Coroutine MoveToPosition(Vector2 position, float speed = 2f, bool smooth = false)
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

            while (root.anchorMin != minAnchorTarget || root.anchorMax != maxAnchorTarget)
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
