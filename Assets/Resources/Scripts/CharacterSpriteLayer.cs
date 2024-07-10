using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Characters
{
    public class CharacterSpriteLayer
    {
        private CharacterManager characterManager => CharacterManager.Instance;

        private const float defaultTransitionSpeed = 3f;
        private float transitionSpeedMultiplier = 1f;

        public int layer { get; private set; } = 0;
        public Image renderer { get; private set; } = null;

        public CanvasGroup rendererCanvasGroup => renderer.GetComponent<CanvasGroup>();

        private List<CanvasGroup> oldRenderers = new List<CanvasGroup>();

        private Coroutine transitioningLayerCoroutine = null;
        private Coroutine levelingAlphaCoroutine = null;
        public bool isTransitioningLayer => transitioningLayerCoroutine != null;
        public bool isLevelingAlpha => levelingAlphaCoroutine != null;

        public CharacterSpriteLayer(Image defaultRenderer, int layer = 0)
        {
            renderer = defaultRenderer;
            this.layer = layer;
        }

        public void SetSprite(Sprite sprite)
        {
            renderer.sprite = sprite;
        }

        public Coroutine TransitionSprite(Sprite sprite, float speed = 2)
        {
            if (sprite == renderer.sprite) return null;

            if (isTransitioningLayer)
            {
                characterManager.StopCoroutine(transitioningLayerCoroutine);
            }

            transitioningLayerCoroutine = characterManager.StartCoroutine(TransitioningSprite(sprite, speed));

            return transitioningLayerCoroutine;
        }

        private IEnumerator TransitioningSprite(Sprite sprite, float speedMultiplier)
        {
            transitionSpeedMultiplier = speedMultiplier;

            Image newRenderer = CreateRenderer(renderer.transform.parent);
            newRenderer.sprite = sprite;

            yield return StartLevelingAlpha();

            transitioningLayerCoroutine = null;
        }

        private Image CreateRenderer(Transform parent)
        {
            Image newRenderer = Object.Instantiate(renderer, parent);

            oldRenderers.Add(rendererCanvasGroup);

            newRenderer.name = renderer.name;
            renderer = newRenderer;
            renderer.gameObject.SetActive(true);

            rendererCanvasGroup.alpha = 0;

            return newRenderer;
        }

        private Coroutine StartLevelingAlpha()
        {
            if (isLevelingAlpha) return levelingAlphaCoroutine;

            levelingAlphaCoroutine = characterManager.StartCoroutine(RunAlphaLeveling());

            return levelingAlphaCoroutine;
        }

        private IEnumerator RunAlphaLeveling()
        {
            while (rendererCanvasGroup.alpha < 1 || oldRenderers.Any(oldCanvasGroup => oldCanvasGroup.alpha > 0))
            {
                float speed = defaultTransitionSpeed * transitionSpeedMultiplier * Time.deltaTime;
                rendererCanvasGroup.alpha = Mathf.MoveTowards(rendererCanvasGroup.alpha, 1, speed);

                for(int i = oldRenderers.Count - 1; i >= 0; i--)
                {
                    CanvasGroup oldCanvasGroup = oldRenderers[i];
                    oldCanvasGroup.alpha = Mathf.MoveTowards(oldCanvasGroup.alpha, 0, speed);

                    if(oldCanvasGroup.alpha <= 0)
                    {
                        oldRenderers.RemoveAt(i);
                        Object.Destroy(oldCanvasGroup.gameObject);
                    }

                    yield return null;
                }
            }

            levelingAlphaCoroutine = null;
        }
    }
}

