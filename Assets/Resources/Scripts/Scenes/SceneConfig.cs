using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Scene Configuration Asset", menuName = "Configuration Assets/Scene Configuration Asset")]
public class SceneConfig : ScriptableObject
{
    public SceneData[] scenes;

    public NPCData[] GetNPCsInScene(string sceneName, string backgroundName)
    {
        sceneName = sceneName.ToLower();
        backgroundName = backgroundName.ToLower();

        for (int i = 0; i < scenes.Length; i++)
        {
            SceneData data = scenes[i];

            if (string.Equals(sceneName, data.sceneName.ToLower()) && string.Equals(backgroundName, data.currentBackgroundName.ToLower()))
            {
                return data.npcsInScene;
            }
        }

        return null;
    }

    public InteractableData[] GetInteractablesInScene(string sceneName, string backgroundName)
    {
        sceneName = sceneName.ToLower();
        backgroundName = backgroundName.ToLower();

        for (int i = 0; i < scenes.Length; i++)
        {
            SceneData data = scenes[i];

            if (string.Equals(sceneName, data.sceneName.ToLower()) && string.Equals(backgroundName, data.currentBackgroundName.ToLower()))
            {
                return data.interactablesInScene;
            }
        }

        return null;
    }

    public BackgroundData[] GetBackgroundsToGoInScene(string sceneName, string backgroundName)
    {
        sceneName = sceneName.ToLower();
        backgroundName = backgroundName.ToLower();

        for (int i = 0; i < scenes.Length; i++)
        {
            SceneData data = scenes[i];

            if (string.Equals(sceneName, data.sceneName.ToLower()) && string.Equals(backgroundName, data.currentBackgroundName.ToLower()))
            {
                return data.backgroundsToGo;
            }
        }

        return null;
    }

    public (bool, Vector2, int) GetPlayerInfo(string sceneName, string backgroundName)
    {
        sceneName = sceneName.ToLower();
        backgroundName = backgroundName.ToLower();

        for (int i = 0; i < scenes.Length; i++)
        {
            SceneData data = scenes[i];

            if (string.Equals(sceneName, data.sceneName.ToLower()) && string.Equals(backgroundName, data.currentBackgroundName.ToLower()))
            {
                return (data.showPlayer, data.playerPosition, data.playerDirection);
            }
        }

        return (false, Vector2.zero, 0);
    }
}

