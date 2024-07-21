using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GraphicPanels;

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
            database.AddCommand("SavePronoun", new Action<string>(SavePronoun));
            database.AddCommand("SetupPixelScene", new Action<string[]>(SetupScene));
            database.AddCommand("HideVN", new Action(() => { SceneManager.Instance.HideVN(); }));
        }

        private static void SetupScene(string[] data)
        {
            if(GraphicPanelManager.Instance.activeGraphicPanel != null)
            {
                GraphicPanelManager.Instance.activeGraphicPanel.Hide();
            }

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

            SceneManager.Instance.SetupScene(firstBackground, position, firstPlayerDirection, endVN);
        }

        private static void SavePronoun(string data)
        {
            if (data == "he")
            {
                TagManager.tags["<subjectPronoun>"] = () => "he";
                TagManager.tags["<objectPronoun>"] = () => "him";
                TagManager.tags["<possessivePronoun>"] = () => "his";
            }
            else if (data == "she")
            {
                TagManager.tags["<subjectPronoun>"] = () => "she";
                TagManager.tags["<objectPronoun>"] = () => "her";
                TagManager.tags["<possessivePronoun>"] = () => "her";
            }
            else if (data == "they")
            {
                TagManager.tags["<subjectPronoun>"] = () => "they";
                TagManager.tags["<objectPronoun>"] = () => "them";
                TagManager.tags["<possessivePronoun>"] = () => "their";
            }
        }
    }
}

