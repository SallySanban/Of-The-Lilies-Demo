using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

namespace Characters
{
    public class SpriteCharacter : Character
    {
        private const string spriteRendererName = "Renderers";
        private CanvasGroup rootCanvasGroup => root.GetComponent<CanvasGroup>();

        public List<CharacterSpriteLayer> layers = new List<CharacterSpriteLayer>();

        private string artAssetsDirectory = "";

        public override bool isCharacterVisible
        { get { return isCharacterShowing || rootCanvasGroup.alpha == 1; }
          set { rootCanvasGroup.alpha = value ? 1 : 0; }
        }

        public SpriteCharacter(string name, CharacterConfigData config, GameObject prefab, string rootAssetsFolder) : base(name, config, prefab)
        {
            rootCanvasGroup.alpha = 0f;
            artAssetsDirectory = rootAssetsFolder + "/Images";

            GetLayers();

            Debug.Log($"Created sprite character: {name}");
        }

        private void GetLayers()
        {
            Transform rendererRoot = animator.transform.Find(spriteRendererName);

            if (rendererRoot == null) return;

            for(int i = 0; i < rendererRoot.transform.childCount; i++)
            {
                Transform child = rendererRoot.transform.GetChild(i);

                Image rendererImage = child.GetComponentInChildren<Image>();

                if(rendererImage != null)
                {
                    CharacterSpriteLayer layer = new CharacterSpriteLayer(rendererImage, i);

                    layers.Add(layer);
                }
            }
        }

        public void SetSprite(Sprite sprite, int layer = 0)
        {
            layers[layer].SetSprite(sprite);
        }

        public Sprite GetSprite(string spriteName)
        {
            return Resources.Load<Sprite>($"{artAssetsDirectory}/{spriteName}");
        }

        public Coroutine TransitionSprite(Sprite sprite, int layer = 0, float speed = 2f)
        {
            CharacterSpriteLayer spriteLayer = layers[layer];

            return spriteLayer.TransitionSprite(sprite, speed);
        }

        public override IEnumerator ShowingOrHiding(bool show)
        {
            float targetAlpha = show ? 1f : 0f;

            CanvasGroup self = rootCanvasGroup;

            while(self.alpha != targetAlpha)
            {
                self.alpha = Mathf.MoveTowards(self.alpha, targetAlpha, 3f * Time.deltaTime);

                yield return null;
            }

            showingCharacterCoroutine = null;
            hidingCharacterCoroutine = null;
        }

        public override void OnReceiveCastingExpression(int layer, string expression)
        {
            Sprite sprite = GetSprite(expression);

            if(sprite == null)
            {
                Debug.LogError($"Sprite {expression} could not be found for character {name}.");
                return;
            }

            TransitionSprite(sprite, layer);
        }
    }
}

