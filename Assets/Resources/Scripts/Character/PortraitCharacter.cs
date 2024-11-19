using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Characters
{
    public class PortraitCharacter : Character
    {
        private const string SPRITE_RENDERER_NAME = "Renderers";
        private CanvasGroup rootCanvasGroup => root.GetComponent<CanvasGroup>();

        public List<CharacterSpriteLayer> layers = new List<CharacterSpriteLayer>();

        string portraitAssetsDirectory;

        public override bool isCharacterVisible
        {
            get { return rootCanvasGroup.alpha == 1; }
            set { rootCanvasGroup.alpha = value ? 1 : 0; }
        }

        public PortraitCharacter(string name, CharacterConfigData config, GameObject prefab, string rootAssetsFolder) : base(name, config, prefab)
        {
            rootCanvasGroup.alpha = 1f;
            portraitAssetsDirectory = FilePaths.FormatPath(FilePaths.portraitAssetsPath, rootAssetsFolder);

            GetLayers();
        }

        private void GetLayers()
        {
            Transform rendererRoot = animator.transform.Find(SPRITE_RENDERER_NAME);

            if (rendererRoot == null) return;

            for (int i = 0; i < rendererRoot.transform.childCount; i++)
            {
                Transform child = rendererRoot.transform.GetChild(i);

                Image rendererImage = child.GetComponentInChildren<Image>();

                if (rendererImage != null)
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
            return Resources.Load<Sprite>($"{portraitAssetsDirectory}/{spriteName}");
        }

        public Coroutine TransitionSprite(Sprite sprite, int layer = 0, float speed = 2f)
        {
            CharacterSpriteLayer spriteLayer = layers[layer];

            return spriteLayer.TransitionSprite(sprite, speed);
        }

        public override void OnReceiveCastingExpression(int layer, string expression)
        {
            Sprite sprite = GetSprite(expression);

            if (sprite == null)
            {
                Debug.LogError($"Sprite {expression} could not be found for character {name}.");
                return;
            }

            TransitionSprite(sprite, layer);
        }
    }
}

