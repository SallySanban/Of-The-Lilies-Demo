//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Characters;
//using System.Linq;

//namespace Commands
//{
//    public class DatabaseExtensionCharacters : CommandDatabaseExtension
//    {
//        private static string xPositionParameter => "-x";
//        private static string yPositionParameter => "-y";
//        private static string speedParameter => "-spd";
//        private static string smoothParameter => "-smooth";
//        private static string emotionParameter => "-exp";
//        private static string immediateParameter => "-i";

//        new public static void Extend(CommandDatabase database)
//        {
//            database.AddCommand("ShowCharacter", new Func<string[], IEnumerator>(Show));
//            database.AddCommand("HideCharacter", new Func<string[], IEnumerator>(Hide));
//            database.AddCommand("MoveCharacter", new Func<string[], IEnumerator>(Move));
//            database.AddCommand("ChangeCharacterEmotion", new Func<string[], IEnumerator>(ChangeEmotion));
//        }

//        private static IEnumerator Show(string[] data)
//        {
//            List<Character> characters = new List<Character>();
//            bool immediate = false;

//            foreach(string s in data)
//            {
//                if(s == immediateParameter)
//                {
//                    break;
//                }

//                Character character = CharacterManager.Instance.GetCharacter(s);

//                if(character != null)
//                {
//                    characters.Add(character);
//                }
//            }

//            if (characters.Count == 0)
//            {
//                yield break;
//            }

//            var parameters = ConvertDataToParameters(data);

//            parameters.TryGetValue(immediateParameter, out immediate, defaultValue: false);

//            foreach(Character character in characters)
//            {
//                if (immediate)
//                {
//                    character.isCharacterVisible = true;
//                }
//                else
//                {
//                    character.Show();
//                }
//            }

//            if (!immediate)
//            {
//                while(characters.Any(c => c.isCharacterShowing))
//                {
//                    yield return null;
//                }
//            }
//        }

//        private static IEnumerator Hide(string[] data)
//        {
//            List<Character> characters = new List<Character>();
//            bool immediate = false;

//            foreach (string s in data)
//            {
//                if (s == immediateParameter)
//                {
//                    break;
//                }

//                Character character = CharacterManager.Instance.GetCharacter(s);

//                if (character != null)
//                {
//                    characters.Add(character);
//                }
//            }

//            if (characters.Count == 0)
//            {
//                yield break;
//            }

//            var parameters = ConvertDataToParameters(data);

//            parameters.TryGetValue("-i", out immediate, defaultValue: false);

//            foreach (Character character in characters)
//            {
//                if (immediate)
//                {
//                    character.isCharacterVisible = false;
//                }
//                else
//                {
//                    character.Hide();
//                }
//            }

//            if (!immediate)
//            {
//                while (characters.Any(c => c.isCharacterHiding))
//                {
//                    yield return null;
//                }
//            }
//        }

//        private static IEnumerator Move(string[] data)
//        {
//            string characterName = data[0];
//            Character character = CharacterManager.Instance.GetCharacter(characterName);

//            if(character == null)
//            {
//                yield break;
//            }

//            float x = 0, y = 0;
//            float speed = 4;
//            bool smooth = true;
//            bool immediate = false;

//            var parameters = ConvertDataToParameters(data);

//            parameters.TryGetValue(xPositionParameter, out x);
//            parameters.TryGetValue(yPositionParameter, out y);
//            parameters.TryGetValue(speedParameter, out speed, defaultValue: 4);
//            parameters.TryGetValue(smoothParameter, out smooth, defaultValue: true);
//            parameters.TryGetValue(immediateParameter, out immediate, defaultValue: false);

//            Vector2 position = new Vector2(x, y);

//            if (immediate)
//            {
//                character.SetPosition(position);
//            }
//            else
//            {
//                yield return character.MoveToPosition(position, speed, smooth);
//            }
//        }

//        private static IEnumerator ChangeEmotion(string[] data)
//        {
//            string characterName = data[0];
//            Character character = CharacterManager.Instance.GetCharacter(characterName);

//            if (character == null)
//            {
//                yield break;
//            }

//            var parameters = ConvertDataToParameters(data);

//            int layer = 1;
//            string expression = "";

//            parameters.TryGetValue(emotionParameter, out expression);

//            character.OnReceiveCastingExpression(layer, expression);
//        }
//    }
//}

