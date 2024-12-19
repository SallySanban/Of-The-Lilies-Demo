using System;
using System.Collections;
using UnityEngine;

namespace Commands
{
    public class DatabaseExtensionScene : CommandDatabaseExtension
    {
        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("ShowScene", new Action<string[]>(ShowScene));
            database.AddCommand("SwitchScene", new Action<string>(SwitchScene));
        }

        private static void ShowScene(string[] data)
        {
            string sceneName = data[0];
            string backgroundName = data[1];

            SceneManager.Instance.CreateScene(sceneName, backgroundName);
        }

        private static void SwitchScene(string data)
        {
            SceneManager.Instance.SwitchScene(data);
        }
    }
}

