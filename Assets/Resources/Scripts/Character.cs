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
        protected Coroutine showingCoroutine, hidingCoroutine;
        public bool isShowing => showingCoroutine != null;
        public bool isHiding => hidingCoroutine != null;
        public virtual bool isVisible => false;

        public Character(string name, CharacterConfigData config, GameObject prefab)
        {
            this.name = name;
            displayName = name;
            this.config = config;

            if(prefab != null)
            {
                GameObject ob = Object.Instantiate(prefab, manager.characterPanel);
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
            if (isShowing) return showingCoroutine;

            if (isHiding)
            {
                manager.StopCoroutine(hidingCoroutine);
            }

            showingCoroutine = manager.StartCoroutine(ShowingOrHiding(true));

            return showingCoroutine;
        }

        public virtual Coroutine Hide()
        {
            if(isHiding) return hidingCoroutine;

            if (isShowing)
            {
                manager.StopCoroutine(showingCoroutine);
            }

            hidingCoroutine = manager.StartCoroutine(ShowingOrHiding(false));

            return hidingCoroutine;
        }

        public virtual IEnumerator ShowingOrHiding(bool show)
        {
            Debug.Log("Show and hide cannot be called from a base character type");
            yield return null;
        }



        public enum CharacterType
        {
            Text,
            Sprite,
            SpriteSheet
        }
    }
}
