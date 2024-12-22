using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Scene Configuration Asset", menuName = "Configuration Assets/Scene Configuration Asset")]
public class SceneConfig : ScriptableObject
{
    public SceneData[] scenes;
    private SceneData[] _runtimeScenes = null;
    private bool initialized = false;

    private void OnEnable()
    {
        _runtimeScenes = null;
        initialized = false;

        InitializeRuntimeScenes();
    }

    private void OnDisable()
    {
        _runtimeScenes = null;
        initialized = false;
    }

    public void InitializeRuntimeScenes()
    {
        if (initialized) return;
        
        if (scenes == null || scenes.Length == 0)
        {
            return;
        }
        
        _runtimeScenes = scenes.Select(scene => scene.Copy()).ToArray();
        initialized = true;
    }

    public NPCData[] GetNPCsInScene(string sceneName, string backgroundName)
    {
        if (!initialized) return null;

        sceneName = sceneName.ToLower();
        backgroundName = backgroundName.ToLower();

        for (int i = 0; i < _runtimeScenes.Length; i++)
        {
            SceneData data = _runtimeScenes[i];
            if (string.Equals(sceneName, data.sceneName.ToLower()) && string.Equals(backgroundName, data.currentBackgroundName.ToLower()))
            {
                return data.npcsInScene?.Select(npc => npc.Copy()).ToArray();
            }
        }
        return null;
    }

    public void RemoveNPCFromScene(string sceneName, string backgroundName, string npcName)
    {
        if (!initialized) return;

        sceneName = sceneName.ToLower();
        backgroundName = backgroundName.ToLower();

        for (int i = 0; i < _runtimeScenes.Length; i++)
        {
            SceneData data = _runtimeScenes[i];
            if (string.Equals(sceneName, data.sceneName.ToLower()) && string.Equals(backgroundName, data.currentBackgroundName.ToLower()))
            {
                _runtimeScenes[i].npcsInScene = _runtimeScenes[i].npcsInScene?
                    .Where(npc => !string.Equals(npc.npcName, npcName))
                    .ToArray();
                break;
            }
        }
    }

    public InteractableData[] GetInteractablesInScene(string sceneName, string backgroundName)
    {
        if (!initialized) return null;

        sceneName = sceneName.ToLower();
        backgroundName = backgroundName.ToLower();

        for (int i = 0; i < _runtimeScenes.Length; i++)
        {
            SceneData data = _runtimeScenes[i];
            if (string.Equals(sceneName, data.sceneName.ToLower()) && string.Equals(backgroundName, data.currentBackgroundName.ToLower()))
            {
                return data.interactablesInScene?.Select(interactable => interactable.Copy()).ToArray();
            }
        }

        return null;
    }

    public void ChangeInteractable(string sceneName, string backgroundName, string interactableName, bool isInteractable)
    {
        if (!initialized) return;

        sceneName = sceneName.ToLower();
        backgroundName = backgroundName.ToLower();

        for (int i = 0; i < _runtimeScenes.Length; i++)
        {
            SceneData data = _runtimeScenes[i];
            if (string.Equals(sceneName, data.sceneName.ToLower()) && string.Equals(backgroundName, data.currentBackgroundName.ToLower()))
            {
                for(int j = 0; j < _runtimeScenes[i].interactablesInScene.Length; j++)
                {
                    if (_runtimeScenes[i].interactablesInScene[j].interactableName.Equals(interactableName))
                    {
                        _runtimeScenes[i].interactablesInScene[j].isInteractable = isInteractable;
                        break;
                    }
                }
            }
        }
    }

    public void RemoveInteractableFromScene(string sceneName, string backgroundName, string interactableName)
    {
        if (!initialized) return;

        sceneName = sceneName.ToLower();
        backgroundName = backgroundName.ToLower();

        for (int i = 0; i < _runtimeScenes.Length; i++)
        {
            SceneData data = _runtimeScenes[i];
            if (string.Equals(sceneName, data.sceneName.ToLower()) && string.Equals(backgroundName, data.currentBackgroundName.ToLower()))
            {
                _runtimeScenes[i].interactablesInScene = _runtimeScenes[i].interactablesInScene?
                    .Where(interactable => !string.Equals(interactable.interactableName, interactableName))
                    .ToArray();
                break;
            }
        }
    }

    public BackgroundData[] GetBackgroundsToGoInScene(string sceneName, string backgroundName)
    {
        if (!initialized) return null;

        sceneName = sceneName.ToLower();
        backgroundName = backgroundName.ToLower();

        for (int i = 0; i < _runtimeScenes.Length; i++)
        {
            SceneData data = _runtimeScenes[i];
            if (string.Equals(sceneName, data.sceneName.ToLower()) && string.Equals(backgroundName, data.currentBackgroundName.ToLower()))
            {
                return data.backgroundsToGo?.Select(bg => bg.Copy()).ToArray();
            }
        }

        return null;
    }

    public (bool, Vector2, int) GetPlayerInfo(string sceneName, string backgroundName)
    {
        if (!initialized) return (false, Vector2.zero, 0);

        sceneName = sceneName.ToLower();
        backgroundName = backgroundName.ToLower();

        for (int i = 0; i < _runtimeScenes.Length; i++)
        {
            SceneData data = _runtimeScenes[i];
            if (string.Equals(sceneName, data.sceneName.ToLower()) && string.Equals(backgroundName, data.currentBackgroundName.ToLower()))
            {
                return (data.showPlayer, data.playerPosition, data.playerDirection);
            }
        }

        return (false, Vector2.zero, 0);
    }
}

