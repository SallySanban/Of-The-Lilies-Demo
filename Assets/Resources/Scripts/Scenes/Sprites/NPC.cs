using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NPC : PixelSprite
{
    public string name;
    public Vector2 lastPosition = Vector2.zero;
    public string lastAnimationState;
    public Vector2 lastInteractablePosition = Vector2.zero;
    public bool appear = true;

    public NPC(GameObject prefab, Transform parent, NPCData npcData)
    {
        root = Object.Instantiate(prefab, Vector2.zero, Quaternion.identity);
        root.transform.SetParent(parent, false);

        animator = root.GetComponentInChildren<Animator>();

        name = npcData.npcName;
        root.name = name;
        lastPosition = npcData.position;
        lastAnimationState = npcData.animationState;
        appear = npcData.appear;

        SetupNPC(appear, lastPosition, lastAnimationState);

        //FOR DEBUGGING PURPOSES
        //root.AddComponent<PositionDebugger>();
    }

    public void SetupNPC(bool appear, Vector2 position, string animationState)
    {
        animator.Play(animationState);
        SetPosition(root, position);

        if (appear)
        {
            root.SetActive(true);
        }
        else
        {
            root.SetActive(false);
        }
    }
}
