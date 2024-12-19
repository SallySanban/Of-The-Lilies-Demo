using System;
using System.Collections;
using Characters;

namespace Commands
{
    public class DatabaseExtensionCharacters : CommandDatabaseExtension
    {
        private static string bodyParameter => "-b";
        private static string emotionParameter => "-e";
        private static string positionParameter => "-p";

        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("ShowCharacterLeft", new Func<string[], IEnumerator>(ShowCharacterLeft));
            database.AddCommand("ShowCharacterRight", new Func<string[], IEnumerator>(ShowCharacterRight));
            database.AddCommand("HideCharacter", new Func<string, IEnumerator>(HideCharacter));
            database.AddCommand("SwitchCharacter", new Func<string[], IEnumerator>(SwitchCharacter));
            database.AddCommand("ChangeBodyEmotion", new Func<string[], IEnumerator>(ChangeBodyEmotion));
        }

        private static IEnumerator ShowCharacterLeft(string[] data)
        {
            Character character = CharacterManager.Instance.GetCharacter(data[0]);

            character.characterPosition = Character.CharacterPosition.Left;

            if (character == null)
            {
                yield break;
            }

            var parameters = ConvertDataToParameters(data);

            string body = "";
            string emotion = "";

            parameters.TryGetValue(bodyParameter, out body);
            parameters.TryGetValue(emotionParameter, out emotion);

            character.SetPosition(character.startingLeftPosition);

            if (!string.IsNullOrEmpty(body)) character.OnReceiveCastingExpression(0, body);
            if (!string.IsNullOrEmpty(emotion)) character.OnReceiveCastingExpression(1, emotion);

            yield return null;

            character.isCharacterVisible = true;
            yield return character.MoveToPosition(character.leftPosition);
        }

        private static IEnumerator ShowCharacterRight(string[] data)
        {
            Character character = CharacterManager.Instance.GetCharacter(data[0]);

            character.characterPosition = Character.CharacterPosition.Right;

            if (character == null)
            {
                yield break;
            }

            var parameters = ConvertDataToParameters(data);

            string body = "";
            string emotion = "";

            parameters.TryGetValue(bodyParameter, out body);
            parameters.TryGetValue(emotionParameter, out emotion);

            character.SetPosition(character.startingRightPosition);

            if (!string.IsNullOrEmpty(body)) character.OnReceiveCastingExpression(0, body);
            if (!string.IsNullOrEmpty(emotion)) character.OnReceiveCastingExpression(1, emotion);

            yield return null;

            character.isCharacterVisible = true;
            yield return character.MoveToPosition(character.rightPosition);
        }

        private static IEnumerator HideCharacter(string data)
        {
            Character character = CharacterManager.Instance.GetCharacter(data);

            if(character.characterPosition == Character.CharacterPosition.Left)
            {
                yield return character.MoveToPosition(character.startingLeftPosition, speed: 2f);
            }
            else
            {
                yield return character.MoveToPosition(character.startingRightPosition, speed: 2f);
            }
        }

        private static IEnumerator SwitchCharacter(string[] data)
        {
            Character currentCharacter = CharacterManager.Instance.GetCharacter(data[0]);
            Character newCharacter = CharacterManager.Instance.GetCharacter(data[1]);

            if (currentCharacter == null || newCharacter == null)
            {
                yield break;
            }

            var parameters = ConvertDataToParameters(data);

            string body = "";
            string emotion = "";
            int position = 0;

            parameters.TryGetValue(bodyParameter, out body);
            parameters.TryGetValue(emotionParameter, out emotion);
            parameters.TryGetValue(positionParameter, out position);

            if (position == 0)
            {
                newCharacter.SetPosition(newCharacter.leftPosition);
                newCharacter.characterPosition = Character.CharacterPosition.Left;
            }
            else
            {
                newCharacter.SetPosition(newCharacter.rightPosition);
                newCharacter.characterPosition = Character.CharacterPosition.Right;
            }

            if (!string.IsNullOrEmpty(body)) newCharacter.OnReceiveCastingExpression(0, body);
            if (!string.IsNullOrEmpty(emotion)) newCharacter.OnReceiveCastingExpression(1, emotion);

            currentCharacter.Hide();

            while (currentCharacter.isCharacterHiding)
            {
                yield return null;
            }

            newCharacter.Show();

            while (newCharacter.isCharacterShowing)
            {
                yield return null;
            }
        }

        private static IEnumerator ChangeBodyEmotion(string[] data)
        {
            string characterName = data[0];
            Character character = CharacterManager.Instance.GetCharacter(characterName);

            if (character == null)
            {
                yield break;
            }

            var parameters = ConvertDataToParameters(data);

            string body = "";
            string emotion = "";

            parameters.TryGetValue(bodyParameter, out body);
            parameters.TryGetValue(emotionParameter, out emotion);

            if (!string.IsNullOrEmpty(body)) character.OnReceiveCastingExpression(0, body);
            if (!string.IsNullOrEmpty(emotion)) character.OnReceiveCastingExpression(1, emotion);
        }
    }
}

