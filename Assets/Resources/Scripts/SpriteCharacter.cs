using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Characters
{
    public class SpriteCharacter : Character
    {
        private CanvasGroup canvasGroup => root.GetComponent<CanvasGroup>();

        public SpriteCharacter(string name, CharacterConfigData config, GameObject prefab) : base(name, config, prefab)
        {
            canvasGroup.alpha = 0f;

            Debug.Log($"Created sprite character: {name}");
        }

        public override IEnumerator ShowingOrHiding(bool show)
        {
            float targetAlpha = show ? 1f : 0f;

            CanvasGroup self = canvasGroup;

            while(self.alpha != targetAlpha)
            {
                self.alpha = Mathf.MoveTowards(self.alpha, targetAlpha, 3f * Time.deltaTime);

                yield return null;
            }

            showingCoroutine = null;
            hidingCoroutine = null;
        }
    }
}

