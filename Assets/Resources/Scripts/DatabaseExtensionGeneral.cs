using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
            database.AddCommand("ShowPixel", new Func<string, IEnumerator>(ShowScene));
            database.AddCommand("ShowLIScreen", new Func<IEnumerator>(ShowLIScreen));
            database.AddCommand("Cutscene", new Action<string>(Cutscene));
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

        private static IEnumerator ShowScene(string data)
        {
            if (bool.TryParse(data, out bool endVN))
            {
                yield return SceneManager.Instance.ShowScene(endVN);
            }
        }

        private static IEnumerator Wait(string data)
        {
            if(float.TryParse(data, out float time))
            {
                yield return new WaitForSeconds(time);
            }
        }

        private static IEnumerator ShowLIScreen()
        {
            BackgroundManager.Instance.RemoveCurrentBackground();
            SpriteManager.Instance.RemoveCurrentPlayer();
            SpriteManager.Instance.RemoveAllSprites(true);

            while(BackgroundManager.Instance.currentBackground != null || SpriteManager.Instance.currentPlayer != null)
            {
                yield return null;
            }

            string liScreenPrefab = "Art/UI/LI Screen/LI Screen";

            GameObject prefab = Resources.Load<GameObject>(liScreenPrefab);

            GameObject liScreen = UnityEngine.Object.Instantiate(prefab, SceneManager.Instance.pixelSceneContainer);

            CanvasGroup liScreenCanvasGroup = liScreen.GetComponent<CanvasGroup>();
            liScreenCanvasGroup.alpha = 0f;

            float fadeSpeed = 3f;

            while (liScreenCanvasGroup.alpha != 1)
            {
                liScreenCanvasGroup.alpha = Mathf.MoveTowards(liScreenCanvasGroup.alpha, 1, fadeSpeed * Time.deltaTime);

                if(liScreenCanvasGroup.alpha == 1)
                {
                    yield return SceneManager.Instance.HideVN();
                }

                yield return null;
            }
        }

        private static void Cutscene(string data)
        {
            if (bool.TryParse(data, out bool cutscene))
            {
                InteractableManager.Instance.cutscene = cutscene;

                InteractableManager.Instance.EnableDisableAllInteractables(cutscene);
            }
        }
    }
}

