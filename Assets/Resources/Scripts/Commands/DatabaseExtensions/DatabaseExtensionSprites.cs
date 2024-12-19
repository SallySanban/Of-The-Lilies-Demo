//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System;
//using GraphicPanels;
//using UnityEngine.UIElements;
//using System.Linq;

//namespace Commands
//{
//    public class DatabaseExtensionSprites : CommandDatabaseExtension
//    {
//        private static string xPositionParameter => "-x";
//        private static string speedParameter => "-spd";

//        new public static void Extend(CommandDatabase database)
//        {
//            database.AddCommand("MoveSprite", new Func<string[], IEnumerator>(MoveSprite));
//            database.AddCommand("HideSprite", new Action<string[]>(HideSprite));
//        }

//        private static IEnumerator MoveSprite(string[] data)
//        {
//            string spriteName = data[0];

//            var parameters = ConvertDataToParameters(data);

//            float position;
//            float speed;

//            parameters.TryGetValue(xPositionParameter, out position);
//            parameters.TryGetValue(speedParameter, out speed);

//            if (spriteName == "Ahlai")
//            {
//                Vector3 playerPosition = SpriteManager.Instance.currentPlayer.root.transform.position;
//                yield return SpriteManager.Instance.currentPlayer.MoveSprite(playerPosition, new Vector3(position, playerPosition.y), speed, isPlayer: true);
//            }
//        }

//        private static void HideSprite(string[] data)
//        {
//            foreach (string s in data)
//            {
//                SpriteManager.Instance.RemoveSprite(s);
//            }
//        }
//    }
//}

