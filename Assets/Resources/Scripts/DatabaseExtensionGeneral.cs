using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GraphicPanels;
using UnityEngine.UIElements;

namespace Commands
{
    public class DatabaseExtensionGeneral : CommandDatabaseExtension
    {
        private static string xPositionParameter => "-x";
        private static string yPositionParameter => "-y";
        private static string directionParameter => "-d";
        private static string backgroundParameter => "-bg";
        private static string endVNParameter => "-vn";

        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("Wait", new Func<string, IEnumerator>(Wait));
            database.AddCommand("SetupPixelScene", new Action<string[]>(SetupScene));
            database.AddCommand("HideVN", new Action<string>(ChangeScene));
        }

        private static void SetupScene(string[] data)
        {
            SceneManager.Instance.sceneName = data[0];

            float firstPlayerXPos = 0f, firstPlayerYPos = 0f;
            string direction = "";
            string firstBackground = "";
            bool endVN = true;

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(xPositionParameter, out firstPlayerXPos);
            parameters.TryGetValue(yPositionParameter, out firstPlayerYPos);
            parameters.TryGetValue(directionParameter, out direction);
            parameters.TryGetValue(backgroundParameter, out firstBackground);
            parameters.TryGetValue(endVNParameter, out endVN, defaultValue: true);

            Vector2 position = new Vector2(firstPlayerXPos, firstPlayerYPos);

            BackgroundConfigData.PlayerDirection firstPlayerDirection = BackgroundConfigData.PlayerDirection.right;

            if (direction == "left")
            {
                firstPlayerDirection = BackgroundConfigData.PlayerDirection.left;
            }

            SceneManager.Instance.SetupBackground(firstBackground, position, firstPlayerDirection, endVN);
        }

        private static void ChangeScene(string data)
        {
            SceneManager.Instance.HideVN();

            if(data != "")
            {
                SceneManager.Instance.sceneName = data;
            }

            SceneManager.Instance.SetupScene();
        }

        private static IEnumerator Wait(string data)
        {
            if(float.TryParse(data, out float time))
            {
                yield return new WaitForSeconds(time);
            }
        }
    }
}

