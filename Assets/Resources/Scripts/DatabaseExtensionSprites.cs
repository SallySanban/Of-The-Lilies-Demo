using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GraphicPanels;
using UnityEngine.UIElements;
using System.Linq;

namespace Commands
{
    public class DatabaseExtensionSprites : CommandDatabaseExtension
    {
        private static string xPositionParameter => "-x";
        private static string speedParameter => "-spd";
        private static string immediateParameter => "-i";

        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("MoveSprite", new Func<string[], IEnumerator>(MoveSprite));
            database.AddCommand("HideSprite", new Func<string[], IEnumerator>(HideSprite));
        }

        private static IEnumerator MoveSprite(string[] data)
        {
            string spriteName = data[0];

            var parameters = ConvertDataToParameters(data);

            float position;
            float speed;

            parameters.TryGetValue(xPositionParameter, out position);
            parameters.TryGetValue(speedParameter, out speed);

            if (spriteName == "Ahlai")
            {
                Vector3 playerPosition = SpriteManager.Instance.currentPlayer.root.transform.position;
                yield return SpriteManager.Instance.currentPlayer.MoveSprite(playerPosition, new Vector3(position, playerPosition.y), speed, isPlayer: true);
            }
        }

        private static IEnumerator HideSprite(string[] data)
        {
            List<PixelSprite> sprites = new List<PixelSprite>();
            bool immediate = false;

            foreach (string s in data)
            {
                if (s == immediateParameter)
                {
                    break;
                }

                PixelSprite sprite = SpriteManager.Instance.GetSprite(s);

                if (sprite != null)
                {
                    sprites.Add(sprite);
                }
            }

            if (sprites.Count == 0)
            {
                yield break;
            }

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue("-i", out immediate, defaultValue: false);

            foreach (PixelSprite character in sprites)
            {
                if (immediate)
                {
                    character.Hide(true);
                }
                else
                {
                    character.Hide();
                }
            }

            if (!immediate)
            {
                while (sprites.Any(c => c.isSpriteHiding))
                {
                    yield return null;
                }
            }
        }
    }
}

