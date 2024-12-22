using UnityEngine;
using System.Linq;

[System.Serializable]
public class SceneData
{
    public string sceneName;
    public string currentBackgroundName;

    public bool showPlayer;
    public Vector2 playerPosition;
    public int playerDirection;

    public NPCData[] npcsInScene;
    public InteractableData[] interactablesInScene;
    public BackgroundData[] backgroundsToGo;

    public SceneData Copy()
    {
        SceneData result = new SceneData();

        result.sceneName = sceneName;
        result.currentBackgroundName = currentBackgroundName;

        result.showPlayer = showPlayer;
        result.playerPosition = playerPosition;
        result.playerDirection = playerDirection;

        result.npcsInScene = npcsInScene?.Select(npc => npc.Copy()).ToArray();
        result.interactablesInScene = interactablesInScene?.Select(interactable => interactable.Copy()).ToArray();
        result.backgroundsToGo = backgroundsToGo?.Select(bg => bg.Copy()).ToArray();

        return result;
    }
}
