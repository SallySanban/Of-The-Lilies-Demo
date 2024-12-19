using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NPCData
{
    public string npcName;
    public Vector2 position;
    public string animationState;
    public string sceneToPlay;

    public NPCData Copy()
    {
        NPCData result = new NPCData();

        result.npcName = npcName;
        result.position = position;
        result.animationState = animationState;
        result.sceneToPlay = sceneToPlay;

        return result;
    }
}
