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
        private static string scaleParameter => "-s";

        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("Wait", new Func<string, IEnumerator>(Wait));
            database.AddCommand("SetupPixelScene", new Action<string[]>(SetupBackground));
            database.AddCommand("HideVN", new Action<string>(SetupScene));
            database.AddCommand("ShowVN", new Action(() => { SceneManager.Instance.ShowVN(); }));
            database.AddCommand("HidePixel", new Action(() => { SceneManager.Instance.HideScene(); }));
            database.AddCommand("ShowPixel", new Func<IEnumerator>(ShowScene));
        }

        private static void SetupBackground(string[] data)
        {
            SceneManager.Instance.sceneName = data[0];

            float firstPlayerXPos = 0f, firstPlayerYPos = 0f;
            float playerScale = 1f;
            string playerDirection = "";
            string firstBackground = "";

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(xPositionParameter, out firstPlayerXPos);
            parameters.TryGetValue(yPositionParameter, out firstPlayerYPos);
            parameters.TryGetValue(scaleParameter, out playerScale, defaultValue: 1f);
            parameters.TryGetValue(directionParameter, out playerDirection);
            parameters.TryGetValue(backgroundParameter, out firstBackground);

            Vector2 position = new Vector2(firstPlayerXPos, firstPlayerYPos);

            Vector2 scale = new Vector2(playerScale, playerScale);

            BackgroundConfigData.PlayerDirection firstPlayerDirection = BackgroundConfigData.PlayerDirection.right;

            if (playerDirection == "left")
            {
                firstPlayerDirection = BackgroundConfigData.PlayerDirection.left;
            }

            SceneManager.Instance.SetupBackground(firstBackground, position, scale, firstPlayerDirection);
        }

        private static void SetupScene(string data)
        {
            SceneManager.Instance.HideVN();

            if(data != "")
            {
                SceneManager.Instance.sceneName = data;
            }

            SceneManager.Instance.SetupScene();
        }

        private static IEnumerator ShowScene()
        {
            yield return SceneManager.Instance.ShowScene();
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

