using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NPC : PixelSprite
{
    public GameObject root = null;

    private SceneManager sceneManager => SceneManager.Instance;

    public string name;
    public Vector2 lastPosition = Vector2.zero;
    public string lastAnimationState;
    public Vector2 lastInteractablePosition = Vector2.zero;

    public NPC(GameObject prefab, Transform parent, NPCData npcData)
    {
        root = Object.Instantiate(prefab, Vector2.zero, Quaternion.identity);
        root.transform.SetParent(parent, false);

        name = npcData.npcName;
        lastPosition = npcData.position;
        lastAnimationState = npcData.animationState;

        SetupNPC(lastPosition, lastAnimationState);

        //FOR DEBUGGING PURPOSES
        //root.AddComponent<PositionDebugger>();
    }

    public void SetupNPC(Vector2 position, string animationState)
    {
        root.GetComponentInChildren<Animator>().Play(animationState);
        SetPosition(root, position);
    }
}
